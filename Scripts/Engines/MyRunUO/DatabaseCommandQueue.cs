using System;
using System.Collections;
using System.Data.Odbc;
using System.Threading;
using Server.Logging;

namespace Server.Engines.MyRunUO
{
	public class DatabaseCommandQueue
	{
		private readonly Queue m_Queue;
		private readonly ManualResetEvent m_Sync;
		private readonly Thread m_Thread;

		private bool m_HasCompleted;

		private readonly string m_CompletionString;
		private readonly string m_ConnectionString;

		public bool HasCompleted => m_HasCompleted;

        public void Enqueue( object obj )
		{
			lock ( m_Queue.SyncRoot )
			{
				m_Queue.Enqueue( obj );
				try{ m_Sync.Set(); }
				catch{}
			}
		}

		public DatabaseCommandQueue( string completionString, string threadName ) : this( Config.CompileConnectionString(), completionString, threadName )
		{
		}

		public DatabaseCommandQueue( string connectionString, string completionString, string threadName )
		{
			m_CompletionString = completionString;
			m_ConnectionString = connectionString;

			m_Queue = Queue.Synchronized( new Queue() );

			m_Queue.Enqueue( null ); // signal connect

			/*m_Queue.Enqueue( "DELETE FROM myrunuo_characters" );
			m_Queue.Enqueue( "DELETE FROM myrunuo_characters_layers" );
			m_Queue.Enqueue( "DELETE FROM myrunuo_characters_skills" );
			m_Queue.Enqueue( "DELETE FROM myrunuo_guilds" );
			m_Queue.Enqueue( "DELETE FROM myrunuo_guilds_wars" );*/

			m_Sync = new ManualResetEvent( true );

			m_Thread = new Thread( Thread_Start );
			m_Thread.Name = threadName;//"MyRunUO Database Command Queue";
			m_Thread.Priority = Config.DatabaseThreadPriority;
			m_Thread.Start();
		}

		private void Thread_Start()
		{
			bool connected = false;

			OdbcConnection connection = null;
			OdbcCommand command = null;
			OdbcTransaction transact = null;

			DateTime start = DateTime.UtcNow;

			bool shouldWriteException = true;

			while ( true )
			{
				m_Sync.WaitOne();

				while ( m_Queue.Count > 0 )
				{
					try
					{
						object obj = m_Queue.Dequeue();

						if ( obj == null )
						{
							if ( connected )
							{
								if ( transact != null )
								{
									try{ transact.Commit(); }
									catch ( Exception commitException )
									{
										ConsoleLog.Write.Warning( "MyRunUO: Exception caught when committing transaction", commitException );
										
										try
										{
											transact.Rollback();
											ConsoleLog.Write.Information( "MyRunUO: Transaction has been rolled back" );
										}
										catch ( Exception rollbackException )
										{
											ConsoleLog.Write.Warning( "MyRunUO: Exception caught when rolling back transaction", rollbackException );
										}
									}
								}

								try{ connection.Close(); }
								catch{}

								try{ connection.Dispose(); }
								catch{}

								try{ command.Dispose(); }
								catch{}

								try{ m_Sync.Close(); }
								catch{}

								ConsoleLog.Write.Information( m_CompletionString, (DateTime.UtcNow - start).TotalSeconds );
								m_HasCompleted = true;

								return;
							}
							else
							{
								try
								{
									connected = true;
									connection = new OdbcConnection( m_ConnectionString );
									connection.Open();
									command = connection.CreateCommand();

									if ( Config.UseTransactions )
									{
										transact = connection.BeginTransaction();
										command.Transaction = transact;
									}
								}
								catch ( Exception e )
								{
									try{ if ( transact != null ) transact.Rollback(); }
									catch{}

									try{ if ( connection != null ) connection.Close(); }
									catch{}

									try{ if ( connection != null ) connection.Dispose(); }
									catch{}

									try{ if ( command != null ) command.Dispose(); }
									catch{}

									try{ m_Sync.Close(); }
									catch{}

									ConsoleLog.Write.Warning( "MyRunUO: Unable to connect to the database", e );
									m_HasCompleted = true;
									return;
								}
							}
						}
						else if ( obj is string )
						{
							command.CommandText = (string)obj;
							command.ExecuteNonQuery();
						}
						else
						{
							string[] parms = (string[])obj;

							command.CommandText = parms[0];

							if ( command.ExecuteScalar() == null )
							{
								command.CommandText = parms[1];
								command.ExecuteNonQuery();
							}
						}
					}
					catch ( Exception e )
					{
						if ( shouldWriteException )
						{
							ConsoleLog.Write.Warning( "MyRunUO: Exception caught in database thread", e );
							shouldWriteException = false;
						}
					}
				}

				lock ( m_Queue.SyncRoot )
				{
					if ( m_Queue.Count == 0 )
						m_Sync.Reset();
				}
			}
		}
	}
}
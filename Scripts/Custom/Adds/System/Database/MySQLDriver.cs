using System.Data;
using System.Data.Odbc;
using System;
using System.Runtime.CompilerServices;
using Server.Logging;

namespace Server.Scripts.Custom.Adds.System.Database
{
    public class MySqlDriver
    {
        public enum AdapterCommandType
        {
            Select,
            Insert,
            Update,
            Delete,
            None

        }

        private string m_Host;
        private string m_Db;
        private string m_User;
        private string m_Password;

        private OdbcConnection m_Connection;
        private OdbcDataAdapter m_Adapter;

        private bool m_Connected = false;

        public MySqlDriver(string host, string db, string user, string password)
        {
            this.m_Host = host;
            this.m_Db = db;
            this.m_User = user;
            this.m_Password = password;
            Connect(host, db, user, password);
        }

        public bool Connected => m_Connected;

        public bool Connect(string host, string db, string user, string password)
        {
            string connectString = "DRIVER={MySQL ODBC 5.1 Driver};" + "SERVER=" + host + ";" + "DATABASE=" + db + ";" + "UID=" + user + ";" + "PASSWORD=" + password + ";" + "OPTION=67108867";
            //ConsoleLog.Write.Information("connecting to db: " + connectString);
            try
            {
                m_Connection = new OdbcConnection(connectString);

                m_Connection.Open();

                //Set the data adapter 
                m_Adapter = new OdbcDataAdapter();

                m_Connected = true;
                //ConsoleLog.Write.Information("connected");
                return true;
            }
            catch (Exception e)
            {
                ConsoleLog.Write.Error($"Cannot connect to the MySQL server.\n{e.StackTrace}");
                m_Connected = false;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Resource Query(string query, AdapterCommandType type)
        {
            //ConsoleLog.Write.Information("Query: " + query);
            Resource datatable = new Resource();

            if (!m_Connected || m_Connection.State != ConnectionState.Open)
            {
                Connect(m_Host, m_Db, m_User, m_Password);
                if (m_Connected)
                {
                    return this.Query(query, type);
                }
                else
                {
                    return datatable;
                }
            }
            try
            {
                if (type == AdapterCommandType.Update || type == AdapterCommandType.Insert || type == AdapterCommandType.Delete)
                {
                    OdbcCommand odbcCommand = m_Connection.CreateCommand();
                    odbcCommand.CommandText = query;
                    odbcCommand.Prepare();
                    odbcCommand.ExecuteNonQuery();

                    return datatable;
                }
                else if (type == AdapterCommandType.Select)
                {

                    OdbcCommand command = m_Connection.CreateCommand();
                    command.CommandText = query;

                    m_Adapter.SelectCommand = command;
                    m_Adapter.Fill(datatable);
                    return datatable;
                }
            }
            catch (InvalidOperationException e) //Database is disconnected
            {
                ConsoleLog.Write.Error("Invalid Operation Exception at Query: " + query);
                ConsoleLog.Write.Error("Message: " + e.Message);
                ConsoleLog.Write.Error(e.StackTrace);
                /*connect(host, db, user, password);
                if(connected)
                	return this.query(query, type);*/
                return datatable;
            }
            catch (OdbcException e) //Database already has the value
            {
                ConsoleLog.Write.Error("OdbcException at Query: " + query);
                ConsoleLog.Write.Error("Message: " + e.Message);
                ConsoleLog.Write.Error(e.StackTrace);
                return datatable;
            }
            return datatable;
        }
    }
}
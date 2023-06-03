using System;
using Server.Accounting;
using Server.Logging;

namespace Server.Misc
{
	public class AccountPrompt
	{
		public static void Initialize()
		{
			if ( Accounts.Count == 0 && !Core.Service )
			{
				ConsoleLog.Write.Information( "This server has no accounts." );
				Console.Write( "Do you want to create the owner account now? (y/n)" );

				if( Console.ReadKey( true ).Key == ConsoleKey.Y )
				{
					ConsoleLog.Write.Information("");

					Console.Write( "Username: " );
					string username = Console.ReadLine();

					Console.Write( "Password: " );
					string password = Console.ReadLine();

					Account a = new Account( username, password );
					a.AccessLevel = AccessLevel.Owner;

					ConsoleLog.Write.Information( "Account created." );
				}
				else
				{
					
					ConsoleLog.Write.Information( "Account not created." );
				}
			}
		}
	}
}
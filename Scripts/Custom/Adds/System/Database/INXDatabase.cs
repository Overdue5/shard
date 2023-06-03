using Server.Mobiles;
using System.Data;
using System.Runtime.CompilerServices;

namespace Server.Scripts.Custom.Adds.System.Database
{
    public static class INXDatabase
    {
        private static MySqlDriver db;

        public static void Initialize()
        {
            //db = new MySqlDriver("localhost", "database", "user", "password");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool CheckMobile(PlayerMobile mob)
        {
            Resource resource = db.Query("SELECT count(*) FROM playermobiles WHERE id = " + (int)mob.Serial, MySqlDriver.AdapterCommandType.Select);
            if (db.Connected)
            {
                DataRow row = resource.nextRow();

                if (row == null)
                    return false;
                if (row[0] != null && row[0].ToString() == "0")
                {
                    InsertNewMobile(mob);
                    return true;
                }
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void InsertNewMobile(PlayerMobile mob)
        {
            db.Query("INSERT INTO playermobiles (id, name, rating, tournamentrating) VALUES (" + (int)mob.Serial + ", '" + mob.Name + "', " + mob.Rating + ", " + mob.TournamentRating + ");", MySqlDriver.AdapterCommandType.Insert);
        }

        public static void ResetDatabase()
        {

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static Resource Query(string query, MySqlDriver.AdapterCommandType commandType)
        {
            return db.Query(query, commandType);
        }
    }
}

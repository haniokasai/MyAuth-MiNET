using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAuth
{
    class mysql
    {
        static MySqlConnection con;

        public static void load()
        {
           con = new MySqlConnection("server=hoge;user=hoge;password=xxxx;database=hoge;");
           con.Open();
           using (MySqlCommand command = con.CreateCommand())
           {
             command.CommandText = "CREATE TABLE IF NOT EXISTS player (name  VARCHAR(25) PRIMARY KEY , passwd TEXT, ip TEXT, uuid TEXT, flogin TEXT, llogin TEXT)";
             command.ExecuteNonQuery();
           }
                Class1._log.Warn("Loaded Mysql.");
        }

        public static void shutdown()
        {
            con.Close();
            Class1._log.Warn("Closed Mysql.");
        }

        public static Dictionary<string,string> get(string name)
        {
            Dictionary<string, string> map = new Dictionary<string, string>;
            try
            {
                using (MySqlCommand command = con.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM player WHERE name = '" + name + "'";
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            map.Add("name",reader["name"].ToString());
                            map.Add("passwd", reader["passwd"].ToString());
                            map.Add("ip", reader["ip"].ToString());
                            map.Add("uuid", reader["uuid"].ToString());
                            map.Add("flogin", reader["flogin"].ToString());
                            map.Add("llogin", reader["llogin"].ToString());
                        }
                        reader.Close();
                    }
                }
                
            }
            catch(MySqlException e)
            {
                Class1._log.Warn("myauth.get: "+e.Message);
            }
                return map;
        }

        public static Boolean settime(String name, int ctime)
        {
            try
            {
                using (MySqlTransaction sqlt = con.BeginTransaction())
                {
                    using (MySqlCommand command = con.CreateCommand())
                    {
                            command.CommandText = "UPDATE player SET  llogin = '" + ctime + "'  WHERE name = '" + name + "'";
                            command.ExecuteNonQuery();
                    }
                    sqlt.Commit();
                }
                return true;
            }
            catch (MySqlException e)
            {
                Class1._log.Warn("myauth.settime: " + e.Message);
                return false;
            }
        }
    }
}

using MiNET;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
namespace MyAuth
{
    class mysql
    {
        static MySqlConnection con;

        public static void load()
        {
            MySqlConnectionStringBuilder conn_string = new MySqlConnectionStringBuilder();
            conn_string.UserID = "root";
            conn_string.Password = "aaaa";
            conn_string.Server = "localhost";
            conn_string.Port = 3306;
            conn_string.Database = "auth";
            
            try
            {
                con = new MySqlConnection(conn_string.ToString());
                con.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "CREATE TABLE IF NOT EXISTS player (name  VARCHAR(25) PRIMARY KEY , passwd  VARCHAR(512), ip VARCHAR(25), uuid VARCHAR(100), flogin  INT, llogin INT)";
                    command.ExecuteNonQuery();
                    command.CommandText = "CREATE TABLE IF NOT EXISTS ban_names (name  VARCHAR(25) PRIMARY KEY , uuid VARCHAR(100), ip VARCHAR(25), time  INT, who  VARCHAR(25))";
                    command.ExecuteNonQuery();
                    command.CommandText = "CREATE TABLE IF NOT EXISTS ban_ips (ip  VARCHAR(25) PRIMARY KEY , name  VARCHAR(25), uuid VARCHAR(100), time  INT, who  VARCHAR(25))";
                    command.ExecuteNonQuery();
                    command.CommandText = "CREATE TABLE IF NOT EXISTS ban_uuids (uuid  VARCHAR(100) PRIMARY KEY , name  VARCHAR(25), ip VARCHAR(25), time  INT, who  VARCHAR(25))";
                    command.ExecuteNonQuery();
                    command.CommandText = "CREATE TABLE IF NOT EXISTS op_ip (ip  VARCHAR(25) PRIMARY KEY ,  time INT, who VARCHAR(25))";
                    command.ExecuteNonQuery();
                    command.CommandText = "CREATE TABLE IF NOT EXISTS op_name (name  VARCHAR(25) PRIMARY KEY , time INT, who VARCHAR(25))";
                    command.ExecuteNonQuery();
                }
            }
            catch(MySqlException e)
            {
                Class1._log.Error(e.Message);
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
            Dictionary<string, string> map = new Dictionary<string, string>();
            try
            {
                using (var command = new MySqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT * FROM player WHERE name = '" + name.ToLower() + "'";
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
                    using (var command = new MySqlCommand())
                    {
                        command.Connection = con;
                        command.CommandText = "UPDATE player SET  llogin = '" + ctime + "'  WHERE name = '" + name.ToLower() + "'";
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

        public static Boolean setuuid(String name, String uuid, String ip)
        {
            try
            {
                using (MySqlTransaction sqlt = con.BeginTransaction())
                {
                    using (var command = new MySqlCommand())
                    {
                        command.Connection = con;
                        command.CommandText = "UPDATE player SET  uuid = '" + uuid + "' ,ip = '" + ip + "'   WHERE name = '" + name.ToLower() + "'";
                        command.ExecuteNonQuery();
                    }
                    sqlt.Commit();
                }
                return true;
            }
            catch (MySqlException e)
            {
                Class1._log.Warn("myauth.setuuid: " + e.Message);
                return false;
            }

        }

        public static void regi(Player player, string hashed)
        { 
            try
            {
                string r = null;
                using (var command = new MySqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT name FROM player WHERE name = '" + player.Username.ToLower() + "'";
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            r = reader["name"].ToString();
                        }
                        reader.Close();
                    }
                }

                if (r == null)
                {
                    using (MySqlTransaction sqlt = con.BeginTransaction())
                    {
                        using (MySqlCommand command = con.CreateCommand())
                        {
                            command.CommandText = "INSERT INTO player (name,passwd,ip,cid,flogin,llogin) VALUES ('" + player.Username.ToLower() + "','" + hashed + "','" + player.EndPoint.Address.MapToIPv4().ToString() + "','" + player.ClientUuid.ToString() + "','" + ToUnixTime(DateTime.Now.ToUniversalTime()) + "','" + ToUnixTime(DateTime.Now.ToUniversalTime()) + "');";
                            command.ExecuteNonQuery();
                        }
                        sqlt.Commit();
                    }
                }
            }
            catch (MySqlException e)
            {
                Class1._log.Warn("myauth.regi: " + e.Message);
            }
        }

        public static Boolean remove(String name)
        {
            try
            {
                string r = null;
                using (var command = new MySqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT name FROM player WHERE name = @name";
                    command.Parameters.Add(new MySqlParameter("name", name.ToLower()));
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            r = reader["name"].ToString();
                        }
                        reader.Close();
                    }
                }

                if (r == null)
                {
                    return false;
                }
                if(r.Equals(name.ToLower()))
                {
                    using (MySqlTransaction sqlt = con.BeginTransaction())
                    {
                        using (MySqlCommand command = con.CreateCommand())
                        {
                            command.CommandText = "DELETE  FROM player WHERE name = @name";
                            command.Parameters.Add(new MySqlParameter("name", name.ToLower()));
                            command.ExecuteNonQuery();
                        }
                        sqlt.Commit();
                    }
                    return true;
                }
            }
            catch (MySqlException e)
            {
                Class1._log.Warn("myauth.remove: " + e.Message);
            }
            return false;
        }


        public static Boolean login(String name, String hashed)
        {
            string p = null;
            try
            {
                using (var command = new MySqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "SELECT passwd FROM player WHERE name = '" + name.ToLower() + "'";
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            p = reader["name"].ToString();
                        }
                        reader.Close();
                    }
                }
            }
            catch (MySqlException e)
            {
                Class1._log.Warn("myauth.login: " + e.Message);
            }
            if (p == null)
            {
                return false;
            }
            if (p.Equals(hashed))
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        public static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public static long ToUnixTime(DateTime targetTime)
        {
            // UTC時間に変換
            targetTime = targetTime.ToUniversalTime();
            // UNIXエポックからの経過時間を取得
            TimeSpan elapsedTime = targetTime - UNIX_EPOCH;
            // 経過秒数に変換
            return (long)elapsedTime.TotalSeconds;
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            // UNIXエポックからの経過秒数で得られるローカル日付
            return UNIX_EPOCH.AddSeconds(unixTime).ToLocalTime();
        }
    


    }
}

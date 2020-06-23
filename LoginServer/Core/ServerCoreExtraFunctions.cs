using System;
using System.Net.Sockets;
using System.Data;
using LoginServer;
using MySql.Data.MySqlClient;

namespace LoginServer.Core
{
    internal partial class ServerCore
    {
        private Character GetCharacter(Socket s)
        {
            for (int i = 0; i < ClientList.Count; i++)
            {
                Socket sc = ClientList[i]._Socket;
                if (sc == s)
                {
                    return ClientList[i]._client;
                }
            }

            return null;
        } // Get Character by Socket

        public ulong ExecuteLogin(string username, string md5password, Database db, ref int Error)
        {
            // Errors
            // 0 - not found
            // 1 - banned
            // 2 - already online
            // 3 - ?
            // 4 - ?
            // 5 - good
            MySqlCommand cmd = db.MYSQL.Connection.CreateCommand();
            ulong accid = 0;
            cmd.CommandText = "SELECT `id`, `accesslevel`, `online` FROM `accounts` WHERE `accounts`.`username`='" + username + "' AND password='" + md5password + "'";
            try
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (reader.GetInt32("accesslevel") >= 100)
                                accid = reader.GetUInt64("id");
                            else
                            {
                                Error = 1; // banned
                                return 0;
                            }
                            if (reader.GetInt32("online") == 0)
                            {
                                Error = 5;
                                return accid;
                            }
                            else
                            {
                                Error = 2;
                                return 0;
                            }

                        }
                    }
                    else
                        Error = 0;
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine("MSG_WARNING", "Login Error " + e.ToString());
                return 0;
            }
            return accid;
        }

        public string GetAllCharactersAndInfo(int accountid, Database db)
        {
            MySqlCommand cmd = db.MYSQL.Connection.CreateCommand();
            MySqlCommand cmd2 = db.MYSQL.Connection.CreateCommand();
            string datareturn = "";
            string itemsreturn = "";
            int charnumber = 0;
            int[] charids = new int[4] { 0, 0, 0, 0 };
            cmd.CommandText = "SELECT * FROM `characters` WHERE `account_id`='" + accountid + "'";
            try
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        //charids = new string[Convert.ToInt32(cmd.ExecuteScalar())];
                        while (reader.Read())
                        {
                            if (charnumber > 4)
                                break;
                            datareturn += reader.GetUInt32("id") + "|";
                            datareturn += reader.GetString("name") + "|";
                            datareturn += reader.GetUInt32("level") + "|";
                            datareturn += reader.GetUInt32("str") + ",";
                            datareturn += reader.GetUInt32("dex") + ",";
                            datareturn += reader.GetUInt32("_int") + ",";
                            datareturn += reader.GetUInt32("luck") + "";
                            charids[charnumber] = reader.GetInt32("id");
                            charnumber++;
                            //Array.Resize(ref charids, charnumber);
                            datareturn += "+";
                        }
                    }
                    else
                        return "";
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine("MSG_WARNING", "Login Error " + e.ToString());
                return "";
            }
            datareturn = datareturn.Remove(datareturn.Length - 1);
            // items now
            int[] slots = new int[8] { 22, 23, 24, 25, 26, 27, 28, 29 };
            for (int i = 0; i < charids.Length; i++)
            {
                string[] items = new string[8] { "0,", "0,", "0,", "0,", "0,", "0,", "0,", "0," };
                if (charids[i] == 0)
                    break;
                try
                {
                    cmd2.CommandText = "SELECT * FROM `character_items` WHERE `ownerID` = '" + charids[i] + "' AND slot >= 22 AND slot <= 29 ORDER BY `slot` DESC";
                    using (MySqlDataReader reader2 = cmd2.ExecuteReader())
                    {
                        if (reader2.HasRows)
                        {
                            while (reader2.Read())
                            {
                                for (int k = 0; k < slots.Length; k++)
                                {
                                    if (slots[k] == reader2.GetInt32("slot"))
                                    {
                                        items[k] = reader2.GetInt32("itemid") + ",";
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("MSG_FATAL", e.ToString());
                }

                items[items.Length - 1] = items[items.Length - 1].Remove(items[items.Length - 1].Length - 1);
                for (int z = 0; z < items.Length; z++)
                    itemsreturn += items[z];
                itemsreturn += ";";
            }
            return datareturn + ";" + itemsreturn;
        }

        private void LaunchDatabaseUpdate()
        {
            MySQLHandler MYSQL = Database.MYSQL;

            if (MYSQL.Connection.State != ConnectionState.Open)
            {
                try
                {
                    MYSQL.Connection.Open();
                }
                catch
                {
                    // err
                }
            }

            MySqlCommand query = MYSQL.Connection.CreateCommand();
            lock(MYSQL)
            {
                query.CommandText = "UPDATE accounts SET `online` = 0";
                query.ExecuteNonQuery();
            }
        }
    }
}

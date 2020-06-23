using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;
using LoginServer.Core;
using MySql.Data.MySqlClient;
using System.Data;
using LoginServer;

namespace LoginServer.ApplicationCore
{
    class ImportJson
    {
        private string File = "";
        private Database Database = null;

        private JsonData itemData;
        private JsonData skillData;
        private JsonData npcData;

        public ImportJson(string file, Database db)
        {
            this.File = file;
            this.Database = db;
        }

        ~ImportJson()
        {
            this.File = "";
            this.Database = null;
        }

        public void ImportItems()
        {
            if(this.File != "" && this.Database != null)
            {
                MySQLHandler MYSQL = Database.MYSQL;
                itemData = JsonMapper.ToObject(this.File);
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
                query.CommandText = "DELETE FROM `items_information`";
                lock(MYSQL)
                {
                    query.ExecuteNonQuery();
                    for (int i = 0; i < itemData.Count; i++)
                    {
                        if(itemData[i]["type"].ToString() == "Weapon")
                            query.CommandText = "INSERT INTO `items_information` (`id`, `type`,`title`,`power`,`defense`,`hp`,`stackable`,`rarity`,`lvl_req`) VALUES ( '" + int.Parse(itemData[i]["id"].ToString()) + "', '" + itemData[i]["type"].ToString() + "', '" + ExtraStaticFunctions.RemoveSpecialCharacters(itemData[i]["title"].ToString()) + "', '" + int.Parse(itemData[i]["weapon"]["attackpower"].ToString()) + "', '0', '0', '0','" + int.Parse(itemData[i]["rarity"].ToString()) + "', '" + int.Parse(itemData[i]["level"].ToString()) + "')";
                        else
                            query.CommandText = "INSERT INTO `items_information` (`id`, `type`,`title`,`power`,`defense`,`hp`,`stackable`,`rarity`,`lvl_req`) VALUES ( '" + int.Parse(itemData[i]["id"].ToString()) + "', '" + itemData[i]["type"].ToString() + "', '" + itemData[i]["title"].ToString() + "', '" + int.Parse(itemData[i]["stats"]["attack"].ToString()) + "', '" + int.Parse(itemData[i]["stats"]["defence"].ToString()) + "', '" + int.Parse(itemData[i]["stats"]["vitality"].ToString()) + "', '" + (bool.Parse(itemData[i]["stackable"].ToString()) ? 1 : 0) + "','" + int.Parse(itemData[i]["rarity"].ToString()) + "', '" + int.Parse(itemData[i]["level"].ToString()) + "')";
                        query.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("MSG_STARTUP", "Successsfully Imported all Items!");
            }
        }

        public void ImportSkills()
        {
            if (this.File != "" && this.Database != null)
            {
                MySQLHandler MYSQL = Database.MYSQL;
                skillData = JsonMapper.ToObject(this.File);
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
                query.CommandText = "DELETE FROM `skills`";
                lock (MYSQL)
                {
                    query.ExecuteNonQuery();
                    for (int i = 0; i < skillData.Count; i++)
                    {
                        query.CommandText = "INSERT INTO `skills` (`id`,`name`,`attack`,`level`,`mana`) VALUES( '" + skillData[i]["id"] + "', '" + ExtraStaticFunctions.RemoveSpecialCharacters(skillData[i]["title"].ToString()) + "', '" + skillData[i]["attack"] + "', '" + skillData[i]["level"] + "', '" + skillData[i]["mana"] + "')";
                        query.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("MSG_STARTUP", "Successsfully Imported all Skills!");
            }
        }

        public void ImportNPCs()
        {
            if (this.File != "" && this.Database != null)
            {
                MySQLHandler MYSQL = Database.MYSQL;
                npcData = JsonMapper.ToObject(this.File);
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
                query.CommandText = "DELETE FROM `npcs`";
                lock (MYSQL)
                {
                    query.ExecuteNonQuery();
                    for (int i = 0; i < npcData.Count; i++)
                    {
                        query.CommandText = "INSERT INTO `npcs` (`id`,`name`,`mapid`,`x`,`y`,`itemids`,`cost`) VALUES( '" + npcData[i]["id"] + "', '" + ExtraStaticFunctions.RemoveSpecialCharacters(npcData[i]["name"].ToString()) + "', '" + int.Parse(npcData[i]["mapid"].ToString()) + "', '" + npcData[i]["x"] + "', '" + npcData[i]["y"] + "', '" + npcData[i]["sellitems"]["itemids"] + "', '" + npcData[i]["sellitems"]["cost"] + "')";
                        query.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("MSG_STARTUP", "Successsfully Imported all NPCs!");
            }
        }
    }
}

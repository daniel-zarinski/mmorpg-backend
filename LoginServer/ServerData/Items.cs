using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LoginServer
{
    public class Item : ICloneable
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Power { get; set; }
        public int Defense { get; set; }
        public int HP { get; set; }
        public bool Stackable { get; set; }
        public int Rarity { get; set; }
        public int LevelRequirement { get; set; }
        public string Type { get; set; }
        public int OwnerID { get; set; }
        public Character Owner { get; set; }
        public int Slot { get; set; }
        public int Amount { get; set; }
        public bool Active { get; set; }
        public int Stat1 { get; set; }
        public int Stat2 { get; set; }
        
        public Item(int id, string title, int power, int defense, int hp, bool stackable, int rarity, int lvl_req, string type)
        {
            this.ID = id;
            this.Title = title;
            this.Power = power;
            this.Defense = defense;
            this.HP = hp;
            this.Stackable = stackable;
            this.Rarity = rarity;
            this.LevelRequirement = lvl_req;
            this.Type = type;
            this.Amount = 1;
            this.OwnerID = 0;
            this.Slot = 0;
            this.Owner = null;
            this.Active = false;
            this.Stat1 = 0;
            this.Stat2 = 0;
        }

        public Item()
        {
            this.ID = -1;
        }

        public object Clone()
        {
            return this.Copy();
        }

        public string ToPacket()
        {
            string ret = "-1";
            if(this.ID > -1)
            {
                ret = this.ID + "+";
                ret += this.Power + "+" + this.Defense + "+" + this.HP + "+" + this.Stat1 + "+" + this.Stat2;
            }

            return ret;
        }
    }

    public class Items
    {
        private List<Item> itemList;

        public Items(MySQLHandler dataBase)
        {
            itemList = new List<Item>();

            if (dataBase.Connection.State != ConnectionState.Open)
            {
                try
                {
                    dataBase.Connection.Open();
                }
                catch
                {
                    //
                }
            }
            
            MySqlCommand query = dataBase.Connection.CreateCommand();
            query.CommandText = "SELECT * FROM `items_information`";
            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Item item = new Item(
                            reader.GetInt32("id"),
                            reader.GetString("title"),
                            reader.GetInt32("power"),
                            reader.GetInt32("defense"),
                            reader.GetInt32("hp"),
                            reader.GetBoolean("stackable"),
                            reader.GetInt32("rarity"),
                            reader.GetInt32("lvl_req"),
                            reader.GetString("type")
                            );
                        itemList.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("There was an error while loading items. \n\n\n" + e.ToString(), "Error while loading Items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

         }

        public List<Item> ItemList
        {
            get { return itemList; }
        }

        public Item GetItemByID(int id)
        {
            Item m = ItemList.Find(x => x.ID == id);
            if (m != null)
                return (Item)m.Clone();

            return new Item();
        }


        public int GetStat(int found = 0)
        {
            // 1-9 : HP
            // 10-19 : Def
            // 20-29 : Attack
            // 30-39 : STR
            // 40-49 : DEX
            // 50-59 : INT
            // 60-69 : LUCK
            // 70-79 : DODGE
            int rand1 = ExtraStaticFunctions.RandomInteger(0, 100); // chance of getting a stat on the item
            int rand2 = ExtraStaticFunctions.RandomInteger(0, 7); // which category of stats
            int rand3 = ExtraStaticFunctions.RandomInteger(0, 100); // how good the stat is

            int statcategory = -1;
            int statvalue = 0;
            if (rand1 < 50 - found)
            {
                statcategory = rand2 * 10;
                if (rand3 >= 50)
                    statvalue = ExtraStaticFunctions.RandomInteger(0, 2); // 50% chance 
                if (rand3 >= 25 && rand3 < 50)
                    statvalue = ExtraStaticFunctions.RandomInteger(3, 5); // 25% chance
                if (rand3 >= 10 && rand3 < 25)
                    statvalue = ExtraStaticFunctions.RandomInteger(6, 7); // 15% chance
                if (rand3 < 10)
                    statvalue = ExtraStaticFunctions.RandomInteger(8, 9);
            }
            return statcategory + statvalue;
        }
    }

}

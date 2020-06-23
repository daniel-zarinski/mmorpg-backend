using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using LoginServer.Core;

namespace LoginServer
{
    public class NPC : ICloneable
    {
        public int ID { get; set; }
        public int MapID { get; set; }
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public sellItem[] SellList { get; set; }
        public class sellItem
        {
            public int ID { get; set; }
            public int Cost { get; set; }
            public Item Item { get; set; }
            public sellItem(int id, int cost, Item item)
            {
                ID = id;
                Cost = cost;
                this.Item = item;
            }
        }
        public NPC(int id, int mapid, string name, float x, float y, Item[] items, string cost)
        {
            this.ID = id;
            this.MapID = mapid;
            this.Name = name;
            int[] tmpcost = Array.ConvertAll(cost.Split(','), s => int.Parse(s));
            SellList = new sellItem[items.Length];
            for (int j = 0; j < items.Length; j++)
            {
                SellList[j] = (new sellItem(j, tmpcost[j], items[j]));
            }

        }
        public NPC()
        {
            this.ID = 0;
        }


        public object Clone()
        {
            return this.Copy();
        }
    }

    public class NPCs
    {
        private List<NPC> npclist;

        public NPCs(MySQLHandler dataBase, Items ItemsDatabase)
        {

            npclist = new List<NPC>();
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
            query.CommandText = "SELECT * FROM `npcs`";

            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int[] tmpcost = Array.ConvertAll(reader.GetString("cost").Split(','), int.Parse);
                        int[] tmpitemids = Array.ConvertAll(reader.GetString("itemids").Split(','), int.Parse);
                        Item[] items = new Item[tmpitemids.Length];
                        for(int i=0; i < tmpitemids.Length; i++)
                        {
                            Item tmpitem = ItemsDatabase.GetItemByID(tmpitemids[i]);
                            items[i] = tmpitem;
                        }
                        NPC npc = new NPC(
                            reader.GetInt32("id"),
                            reader.GetInt32("mapid"),
                            reader.GetString("name"),
                            reader.GetFloat("x"),
                            reader.GetFloat("y"),
                            items,
                            reader.GetString("cost")
                            );
                        npclist.Add(npc);
                    }
                }
            }
            catch
            {
                //
            }

        }

        public NPC GetNPCByID(int id)
        {
            NPC n = NPCsList.Find(x => x.ID == id);
            if (n != null)
                return (NPC)n.Clone();

            return new NPC();
        }


        public List<NPC> NPCsList
        {
            get
            {
                return npclist;
            }
            private set { }
        }
    }
}

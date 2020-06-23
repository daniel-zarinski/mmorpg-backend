using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace LoginServer
{
    public class Monster : ICloneable
    {
        public int ID { get; set; }
        public int MonsterID { get; private set; }
        public string Title { get; private set; }
        public int Level { get; private set; }
        public int Attack { get; private set; }
        public int MaxHP { get; private set; }
        public int EXP { get; private set; }
        public int RespawnTime { get; set; }
        public int MapID { get; set; }
        public int WaveID { get; set; }
        public Position position { get; set; }
        public int[] DropIDs { get; set; }
        public int[] DropProbability { get; set; }
        public uint Gold { get; set; }
        public class Position
        {
            public float XCoord { get; set; }
            public float YCoord { get; set; }

            public Position()
            {
                this.XCoord = 0;
                this.YCoord = 0;
            }

            public Position(float x, float y)
            {
                this.XCoord = x;
                this.YCoord = y;
            }
        }

        // constructor
        public Monster()
        {
            MonsterID = -1;
        }

        public Monster(int monid, string title, int level, int attack, int maxhp, int exp, int respawntime, int[] dropid, int[] dropprobability, uint gold)
        {
            this.MonsterID = monid;
            this.Title = title;
            this.Level = level;
            this.Attack = attack;
            this.MaxHP = maxhp;
            this.EXP = exp;
            this.RespawnTime = respawntime;
            this.DropIDs = dropid;
            this.DropProbability = dropprobability;
            this.Gold = gold;
            this.ID = -1;
            this.MapID = -1;
            this.position = new Position();
            this.WaveID = -1;
        }



        
        public Item[] GetDrops(Items itemdb)
        {
            Monster m = this;
            int[] ReturnDrop = new int[m.DropIDs.Length];
            for (int i = 0; i < m.DropIDs.Length; i++)
            {
                if (ExtraStaticFunctions.RandomInteger(0, 100) < m.DropProbability[i])
                {
                    ReturnDrop[i] = m.DropIDs[i];
                }
                else
                    ReturnDrop[i] = 0;
            }

            Item[] retDrop = new Item[ReturnDrop.Length];
            for(int i=0; i < ReturnDrop.Length; i++)
            {
                if (ReturnDrop[i] == 0)
                {
                    retDrop[i] = new Item();
                    continue;
                }
                Item tmpitem = itemdb.GetItemByID(ReturnDrop[i]);
                if(tmpitem.ID > -1)
                {
                    double tmpPower = tmpitem.Power * ExtraStaticFunctions.RandomInteger(80, 120) / 100;
                    double tmpDef = tmpitem.Defense * ExtraStaticFunctions.RandomInteger(70, 130) / 100;
                    double tmpHP = tmpitem.HP * ExtraStaticFunctions.RandomInteger(80, 120) / 100;
                    int tmpStat1 = itemdb.GetStat();
                    int tmpStat2 = 0;
                    tmpitem.Power = (int)Math.Round(tmpPower, MidpointRounding.AwayFromZero);
                    tmpitem.Defense = (int)Math.Round(tmpDef, MidpointRounding.AwayFromZero);
                    tmpitem.HP = (int)Math.Round(tmpHP, MidpointRounding.AwayFromZero);
                    tmpitem.Stat1 = tmpStat1 < 0 ? 0 : tmpStat1;
                    if (tmpStat1 < 0)
                        tmpStat2 = itemdb.GetStat();
                    else
                        tmpStat2 = itemdb.GetStat(15);

                    tmpitem.Stat2 = tmpStat2 < 0 ? 0 : tmpStat2;

                    retDrop[i] = tmpitem;
                }
            }
            return retDrop;
        }

        public string GetDropPack(Items itemdb)
        {
            Item[] drop = GetDrops(itemdb);
            if (drop.Length < 1)
                return "";
            string returndroppak = "";
            for (int i = 0; i < drop.Length; i++)
            {
                if (drop[i].ID > 0)
                {
                    returndroppak += drop[i].ToPacket() + ",";
                }
                else
                    returndroppak += "0,";
            }

            if(returndroppak.Length > 1)
                returndroppak = returndroppak.Remove(returndroppak.Length - 1);
            return returndroppak;
        }

        public uint GetMoney()
        {
            Monster m = this;
            uint g = (uint)( m.Gold * ExtraStaticFunctions.RandomInteger(60, 140) / 100 );
            return g;
        }

        public object Clone()
        {
            return this.Copy();
        }
    }

    public class Monsters
    {
        private List<Monster> monlist;
        
        public Monsters(MySQLHandler dataBase)
        {
            monlist = new List<Monster>();

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
            query.CommandText = "SELECT * FROM `monsters`";

            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int[] dropids = Array.ConvertAll(reader.GetString("drop_item_id").Split(','), int.Parse);
                        int[] dropprobability = Array.ConvertAll(reader.GetString("drop_probability").Split(','), int.Parse);
                        Monster mon = new Monster(
                            reader.GetInt32("id"),
                            reader.GetString("title"),
                            reader.GetInt32("level"),
                            reader.GetInt32("attack"),
                            reader.GetInt32("maxhp"),
                            reader.GetInt32("exp"),
                            reader.GetInt32("respawntime"),
                            dropids,
                            dropprobability,
                            reader.GetUInt32("gold")
                            );
                        monlist.Add(mon);
                    }
                }
            }
            catch
            {
                //
            }


        }

        public Monster GetMonsterByID(int id)
        {
            Monster m = MonsterListDB.Find(x => x.MonsterID == id);
            if (m != null)
                return (Monster)m.Clone();

            return new Monster();
        }

        public List<Monster> MonsterListDB
        {
            get
            {
                List<Monster> copy = new List<Monster>(monlist);
                return copy;
            }
            private set { }
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using MySql.Data.MySqlClient;
using LoginServer.Static;
namespace LoginServer
{
    public class Character : Creature
    {
        private int exp;
        private ulong gold;
        private ulong damage;
        private uint statpoints;
        private uint skillpoints;
        private uint attack;
        private uint defense;
        private int accID;
        private int isConnected;

        private Socket _socketClient;
        private string _movementpak = "0;0;0;0;0";
        private string shoot = "";
        private MySQLHandler MYSQL;
        private int _wave = 1;
        private int _maxwave;
        private string hash;

        private Items ItemsDB; // should remove
        private Skills AllSkills;
        private List<Skill> _skillList;
        private List<Item> _equipmentList;
        public const int MAX_SLOTS = 45;

        public Character()
        {
            id = 0;
            _socketClient = null;
            accID = -1;
        }

        public Character (Socket s)
        {
            id = 0;
            _socketClient = s;
        }

        ~Character()
        {
            id = 0;
            _socketClient = null;
            accID = -1;
        }

        public Character(ulong playerID, MySQLHandler GlobalMysql, Socket s, Items itemsDB)
        {
            MYSQL = GlobalMysql;
            // Equipment and Storage goes here.
            // equipment = new CharacterEquipment(playerID, GlobalMysql);
            // storage = new CharacterStorage(playerID, GlobalMysql);
            _skillList = new List<Skill>();
            AllSkills = new Skills(MYSQL);
            ItemsDB = itemsDB;
            _socketClient = s;

            // Entire item list for all items in database
            _equipmentList = new List<Item>();

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

            query.CommandText = "SELECT * FROM `characters` WHERE `characters`.`id` = " + playerID;
            // 0,10;     1,30;2,30;
            lock (GlobalMysql)
            {
                try
                {
                    using (MySqlDataReader reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            name = reader.GetString("name");
                            level = reader.GetInt32("level");
                            id = reader.GetUInt64("id");
                            str = reader.GetUInt32("str");
                            dex = reader.GetUInt32("dex");
                            _int = reader.GetUInt32("_int");
                            luck = reader.GetUInt32("luck");
                            maxhp = reader.GetUInt32("maxhp");
                            maxmp = reader.GetUInt32("maxmp");
                            curhp = reader.GetUInt32("curhp");
                            curmp = reader.GetUInt32("curmp");
                            mapid = reader.GetInt32("mapid");
                            _maxwave = reader.GetInt32("wavenumber");
                            skillpoints = reader.GetUInt32("skillpoints");
                            statpoints = reader.GetUInt32("statpoints");
                            exp = reader.GetInt32("exp");
                            gold = reader.GetUInt32("gold");
                            hash = MyEncryption.CreateCharacterHash(reader.GetString("account_id"));
                            this.Id = id;
                            accID = reader.GetInt32("account_id");

                            // skill stuff
                            string[] tmpactiveskill = reader.GetString("active_skill").Split(',');
                            int[] tmpactiveskills = new int[4];
                            string[] skillids = reader.GetString("skill_id").Split(',');
                            int[] skillidlist = new int[11];
                            int skillCounter = 0;
                            string[] skilllevel = reader.GetString("skill_level").Split(',');
                            int[] skilllevels = new int[11];
                            foreach (string _skillid in skillids)
                            {
                                if (skillCounter >= 10)
                                    break;
                                int tempID;
                                int val = Int32.TryParse(_skillid, out tempID) ? tempID : 0;
                                if (val == 0)
                                    continue;
                                skillidlist[skillCounter] = val;
                                skillCounter++;
                            }
                            skillCounter = 0;
                            foreach (string _skilllevel in skilllevel)
                            {
                                if (skillCounter >= 10)
                                    break;
                                int tempID;
                                int val = Int32.TryParse(_skilllevel, out tempID) ? tempID : 0;
                                if (val == 0)
                                    continue;
                                skilllevels[skillCounter] = val;
                                skillCounter++;
                            }
                            skillCounter = 0;
                            foreach (string _tmpactiveskill in tmpactiveskill)
                            {
                                if (skillCounter > 3)
                                    break;
                                int tempID;
                                int val = Int32.TryParse(_tmpactiveskill, out tempID) ? tempID : 0;
                                if (val == 0)
                                    continue;
                                tmpactiveskills[skillCounter] = val;
                                skillCounter++;
                            }

                            for (int i = 0; i < skillidlist.Count(); i++)
                            {
                                Skill skill = AllSkills.GetSkillByID(skillidlist[i]);
                                if (skill == null)
                                    continue;
                                for (int j = 0; j < tmpactiveskills.Count(); j++)
                                {
                                    if (tmpactiveskills[j] == skill.Id)
                                        skill.Active = true;
                                }
                                skill.Level = skilllevels[i];
                                //Skill skill = new Skill(skillidlist[i], AllSkills.SkillList[j].Attack, skilllevels[i], AllSkills.SkillList[j].Name);
                                _skillList.Add(skill);
                            }
                        }
                    }
                }
                catch (MySqlException e)
                {
                    MessageBox.Show(e.ToString(), "Server can't start.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            // Items Now
            // fill up the list
            for (int i = 0; i < MAX_SLOTS; i++)
                _equipmentList.Add(new Item());
            query.CommandText = "SELECT * FROM `character_items` WHERE `ownerID` = " + (int) id + " ORDER BY slot DESC";
            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Item tmpItem = new Item();
                        tmpItem = ItemsDB.GetItemByID(reader.GetInt32("itemid"));
                        tmpItem.Slot = reader.GetInt32("slot");
                        tmpItem.Amount = reader.GetInt32("amount");
                        tmpItem.Power = reader.GetInt32("attack");
                        tmpItem.Defense = reader.GetInt32("defense");
                        tmpItem.HP = reader.GetInt32("hp");
                        tmpItem.Stat1 = reader.GetInt32("extra_stat1");
                        tmpItem.Stat2 = reader.GetInt32("extra_stat2");
                        if (tmpItem.ID != -1)
                        {
                            //_equipmentList.Add(tmpItem);
                            _equipmentList[tmpItem.Slot] = tmpItem;
                        }
                        else
                        {
                            MessageBox.Show("Invalid item","Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Error while loading items for charid = " + id + " \n\n\n" + e.ToString(), "Charater.cs Error - Failed to load items", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            LastSave = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            
        }

        public void AddItemOLD(Item item)
        {
            Item tmpItem = _equipmentList.Find(pos => pos.ID == item.ID && pos.Type == item.Type);

            if (MYSQL.Connection.State != ConnectionState.Open)
            {
                try
                {
                    MYSQL.Connection.Open();
                }
                catch (Exception e)
                {
                    MessageBox.Show("MYSQL Connection error" + e.ToString());
                    return;
                }
            }

            MySqlCommand query = MYSQL.Connection.CreateCommand();

            if(tmpItem == null)
            {
                item.Slot = _equipmentList.Count;
                _equipmentList.Add(item);
                query.CommandText = "INSERT INTO `character_items` (`ownerID`,`itemid`,`slot`,`amount`,`attack`,`defense`,`hp`,`extra_stat1`,`extra_stat2`) VALUES ('" + this.id + "', '" + item.ID + "', '" + item.Slot + "')"; 
                query.ExecuteNonQuery();
            }
            else
            {
                if (tmpItem.Stackable)
                {
                    tmpItem.Amount++;
                    query.CommandText = "UPDATE `character_items` SET `amount` = '" + tmpItem.Amount + "' WHERE `ownerID` = '" + this.id + "' AND `itemid` = '" + tmpItem.ID + "'";
                    query.ExecuteNonQuery();
                }
                else
                {
                    item.Slot = _equipmentList.Count;
                    _equipmentList.Add(item);
                    query.CommandText = "INSERT INTO `character_items` (`ownerID`,`itemid`,`slot`) VALUES ('" + this.id + "', '" + item.ID + "', '" + item.Slot + "')";
                    query.ExecuteNonQuery();
                }
            }
            
        }

        public int GetEmptySlot()
        {
            for(int i=0; i < EquipmentList.Count; i++)
            {
                if (EquipmentList[i].ID < 0)
                    return i;
            }
            
            return EquipmentList.Count;
        }

        public void SaveItems()
        {
            // delete all from mysql
            if (MYSQL.Connection.State != ConnectionState.Open)
            {
                try
                {
                    MYSQL.Connection.Open();
                }
                catch (Exception e)
                {
                    MessageBox.Show("MYSQL Connection error" + e.ToString());
                    return;
                }
            }
            MySqlCommand query = MYSQL.Connection.CreateCommand();
            query.CommandText = "DELETE FROM character_items WHERE `ownerID` = '" + this.Id + "'";

            lock (MYSQL)
            {
                query.ExecuteNonQuery();
                for (int i = 0; i < EquipmentList.Count; i++)
                {
                    if (EquipmentList[i].ID < 0)
                        continue;
                    if (MYSQL.Connection.State == ConnectionState.Open)
                    {
                        query.CommandText = "INSERT INTO `character_items` (`ownerID`,`itemid`,`slot`,`amount`,`attack`,`defense`,`hp`,`extra_stat1`,`extra_stat2`) VALUES ('" + this.id + "', '" + EquipmentList[i].ID + "', '" + EquipmentList[i].Slot + "', '" + EquipmentList[i].Amount + "', '" + EquipmentList[i].Power + "', '" + EquipmentList[i].Defense + "', '" + EquipmentList[i].HP + "', '" + EquipmentList[i].Stat1 + "', '" + EquipmentList[i].Stat2 + "')";
                        query.ExecuteNonQuery();
                    }
                }
            }
        }

        public bool LearnSkill(int skillid, int slot)
        {
            if (this.SkillPoints < 0)
                return false;
            for(int i=0; i < Skills.Count; i++)
            {
                if (Skills[i].Id == skillid)
                    return false;
            }

            Skill newskill = AllSkills.GetSkillByID(skillid);
            if (newskill == null)
                return false;

            newskill.Level = 1;
            newskill.Active = false;
            Skills.Add(newskill);
            this.SkillPoints--;
            return true;
        }

        public void SaveSkills()
        {
            if (MYSQL.Connection.State != ConnectionState.Open)
            {
                try
                {
                    MYSQL.Connection.Open();
                }
                catch (Exception e)
                {
                    MessageBox.Show("MYSQL Connection error" + e.ToString());
                    return;
                }
            }
            string tmpskillstring = "";
            string tmpskilllevelstring = "";
            string tmpactivestring = "";
            for(int i=0; i < this.Skills.Count; i++)
            {
                tmpskillstring += this.Skills[i].Id + ",";
                tmpskilllevelstring += this.Skills[i].Level + ",";
                if (this.Skills[i].Active)
                    tmpactivestring += this.Skills[i].Id + ",";
            }
            tmpskillstring = tmpskillstring.Remove(tmpskillstring.Length - 1);
            tmpskilllevelstring = tmpskilllevelstring.Remove(tmpskilllevelstring.Length - 1);
            if (tmpactivestring.Length > 1)
                tmpactivestring = tmpactivestring.Remove(tmpactivestring.Length - 1);
            else
                tmpactivestring = 0.ToString();
            try
            {
                MySqlCommand query = MYSQL.Connection.CreateCommand();
                query.CommandText = "UPDATE `characters` SET `skillpoints` = '" + this.SkillPoints + "', `skill_id` = '" + tmpskillstring + "', `skill_level` = '" + tmpskilllevelstring + "', `active_skill` = '" + tmpactivestring + "' WHERE `id` = '" + this.Id + "'";
                query.ExecuteNonQuery();
                
            }
            catch(Exception e)
            {
                MessageBox.Show("oops" + e.ToString());
            }
            
        }

        public void SaveStats()
        {
            lock (MySQLHandler.LockSQL)
            {
                if (MYSQL.Connection.State != ConnectionState.Open)
                {
                    try
                    {
                        MYSQL.Connection.Open();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("MYSQL Connection error" + e.ToString());
                        return;
                    }
                }

                try
                {
                    MySqlCommand query = MYSQL.Connection.CreateCommand();
                    query.CommandText = "UPDATE `characters` SET `statpoints` = '" + this.StatPoints + "', `str` = '" + this.STR + "', `dex` = '" + this.DEX + "', `_int` = '" + this.INT + "', `luck` = '" + this.Luck + "', `exp` = '" + this.EXP + "', `level` = '" + this.Level + "', `statpoints` = '" + this.StatPoints + "', `skillpoints` = '" + this.SkillPoints + "', `maxhp` = '" + this.MaxHP + "', `maxmp` = '" + this.MaxMP + "', `gold` = '" + this.Gold + "' WHERE `id` = '" + this.Id + "'";
                    query.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    MessageBox.Show("SaveStats Error " + e.ToString());
                }
            }
        }

        public string Packet()
        {
            string tmp = "";
            //int id, string name, int maxhealth, int maxmp, int level, int skillpoints, int str, int dex, int luck, int _int, int statpoints, int exp
            tmp = this.Id + ";";
            tmp += this.Name + ";";
            tmp += this.MaxHP + ";";
            tmp += this.MaxMP + ";";
            tmp += this.Level + ";";
            tmp += this.SkillPoints + ";";
            tmp += this.STR + ";";
            tmp += this.DEX + ";";
            tmp += this.Luck + ";";
            tmp += this.INT + ";";
            tmp += this.StatPoints + ";";
            tmp += this.EXP + ";";
            tmp += this.Gold + ";";

            return tmp;
        }

        public string ItemsPacket()
        {
            string items = "";
            //EquipmentList.OrderBy(t => t.Slot).ToList();
            for(int j=0; j < EquipmentList.Count; j++)
            {
                items += (EquipmentList[j].ToPacket()) == "" ? EquipmentList[j].ID.ToString() : EquipmentList[j].ToPacket() + ",";
            }
            return items.Remove(items.Length - 1);
        }

        public string EquippedItemsPacket()
        {
            string items = "";
            for (int j = EquipmentList.Count-10; j < EquipmentList.Count; j++)
            {
                items += (EquipmentList[j].ToPacket()) == "" ? EquipmentList[j].ID.ToString() : EquipmentList[j].ToPacket() + ",";
            }
            return items.Remove(items.Length - 1);
        }

        public string ActiveSkillsPackekt()
        {
            string skills = "";
            for(int i=0; i < Skills.Count; i++)
            {
                if (Skills[i].Active)
                    skills += Skills[i].Id + ",";
            }

            return skills;
        }

        public string ClientConnectedPacket()
        {
            return this.Id + ";" + Name + ";" + Level + ";" + MaxHP + ";" + MaxMP + ";" + X + ";" + Y + ";" + EquippedItemsPacket() + ";" + ActiveSkillsPackekt() + ";" + STR + ";" + DEX + ";" + INT + ";" + Luck;

        }
        public Item GetItem(int slot)
        {
            return EquipmentList[slot];
        }

#region Acc

    public string CharHash
        {
            get { return hash; }
        }

    public List<Skill> Skills
        {
            get { return _skillList; }
        }

    public List<Item> EquipmentList
        {
            get
            {
                return _equipmentList;
            }

        }

    public ulong Id
    {
        get { return id;}
        set { id = value; }
    }

    public int WaveNumber
        {
            get { return _wave; }
            set { _wave = value; }
        }

        public int MaxWaveNumber
        {
            get { return _maxwave; }
            set { _maxwave = value; }
        }
    public string movementPak
    {
        get { return _movementpak; }
        set { _movementpak = value;  }
    }
   

    public string Shoot
    {
        get
        {
            return shoot;
        }
        set
        {
            shoot = value;
        }
    }

    public Socket socketClient
    {
        get { return _socketClient; }
        set { _socketClient = value; }
    }

    public uint DEX
    {
        get
        {
            return dex;
        }
        set
        {
            dex = value;
        }
    }

    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
        }
    }

    public uint Luck
    {
        get
        {
            return luck;
        }
        set
        {
            luck = value;
        }
    }

    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    public uint INT
    {
        get
        {
            return _int;
        }
        set
        {
            _int = value;
        }
    }

    public uint STR
    {
        get
        {
            return str;
        }
        set
        {
            str = value;
        }
    }

    public int EXP
    {
        get
        {
            return exp;
        }
        set
        {
            exp = value;
        }
    }

    public ulong Gold
    {
        get
        {
            return gold;
        }
        set
        {
            gold = value;
        }
    }

    public ulong Damage
{
    get
    {
        return damage;
    }
    set
    {
        damage = value;
    }
}
        
    public uint StatPoints
    {
        get { return statpoints; }
        set { statpoints = value; }
    }

    public uint SkillPoints
    {
        get { return skillpoints; }
        set { skillpoints = value; }
    }

    public uint CurHP
    {
        get { return curhp; }
        set { curhp = value; }
    }

    public uint CurMP
    {
        get { return curmp; }
        set { curmp = value; }
    }

    public uint MaxHP
    {
        get { return maxhp; }
        set { maxhp = value; }
    }

    public uint MaxMP
    {
        get { return maxmp; }
        set { maxmp = value; }
    }

    public int MapID
    {
        get { return mapid; }
        set { mapid = value; }
    }

    public long LastSave { get; set; }
    public int AccountID
        {
            get { return accID; }
            set { accID = value; }
        }

    public float X
    {
        get { return Xpos; }
        set { Xpos = value; }
    }

    public float Y
    {
        get { return Ypos; }
        set { Ypos = value; }
    }
#endregion Acc

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using Commands;
using LoginServer.Static;
/*
namespace LoginServer
{
    public partial class frmLoginServer
    {
        public void LOGIN_Packet(Socket s, string username, string password)
        {
            ulong userID = ExecuteLogin(username, password);
            if (userID == 0)
            {
                AddLog("MSG_WARNING", "Login attempt failed. username=" + username + " password=" + password + "");
                SendData(s, "LOGIN;0");
                return;
            }
            else
            {
                AddLog("MSG_INFO", "Account logged in UserID = " + userID);
            }
            SendData(s, "LOGIN;" + GetAllCharactersAndInfo((int)userID) + MyEncryption.CreateCharacterHash(userID.ToString()));
        }

        public void GETWORLD_Packet(Socket s, string[] data)
        {
            //AddLog("MSG_INFO", "User selected characterID = " + textReceived);
            Character thisclient;
            thisclient = new Character(Convert.ToUInt64(data[1]), MYSQL, s, ItemDatabase);
            if (thisclient.CharHash != data[2])
            {
                AddLog("MSG_INFO", "Hash code = " + data[2]);
                return;
            }
            AddLog("MSG_INFO", "User selected CharacterName = " + thisclient.Name);
            int index = -1;
            lock(ClientList)
            {
                for(int i=0; i< ClientList.Count; i++)
                {
                    if (ClientList[i]._Socket.RemoteEndPoint.ToString().Equals(s.RemoteEndPoint.ToString()))
                    {
                        ClientList[i] = new ClientList(thisclient);
                        index = i;
                    }
                }
            }

            //ClientList.Add(new ClientList(thisclient));
            if(index < 0)
            {
                AddLog("MSG_FATAL", "CLIENT COULD NOT BE FOUND IN ClientList. WTF?");
            }

            if (thisclient != null && thisclient.Id > 0)
            {
                lbCharacters.Items.Add(thisclient.Name);
                string temp = "0;" + thisclient.Level + ";25;";
                for (int i = 0; i < thisclient.Skills.Count; i++)
                {
                    temp += thisclient.Skills[i].Id + ";" + thisclient.Skills[i].Level + ";" + thisclient.Skills[i].Active + ";";
                }
                SendData(s, "SKILLS;" + temp);
                SendData(s, "INVENTORY;" + thisclient.ItemsPacket());
                SendData(s, ClientCmd.GETWORLD + ";" + thisclient.Packet() + ";");
                MapList.Add(new MapList(thisclient, 0));
                AddLog("MSG_FATAL", thisclient.Packet());

                Maps.CreateNewWave(thisclient, waves);

                //SendToAllConnected("SPAWNM;3;1;"+thisclient.WaveNumber);
                AddLog("MSG_INFO", "Character information sent.");
            }
            else
                AddLog("MSG_FATAL", "Character information could not be sent. Unknown charatcer.");
            thisclient = null;
        }

        public void MOVE_Packet(Socket s, string[] data)
        {
            // structure: x - y- vx - vy - facingright
            Character thisclient = GetCharacter(s);
            if (thisclient == null)
                return; // might crash
            if (data.Length < 5)
            {
                AddLog("MSG_FATAL", "Movement Packet incorrect size.");
                return;
            }
            thisclient.movementPak = data[1] + ";" + data[2] + ";" + data[3] + ";" + data[4];
            for (int j = 0; j < ClientList.Count; j++)
            {
                Socket sj = ClientList[j]._Socket;
                if (sj == s)
                    continue;
                SendData(sj, "RMOVE;" + thisclient.movementPak);
            }
        }

        public void SHOOT_Packet(Socket s, string[] data)
        {
            //ulong id = character.Id;
            //AddLog("MSG_FATAL", "ID = " + id);
            for (int i = 0; i < ClientList.Count; i++)
            {
                Socket sc = ClientList[i]._Socket;
                if (sc == s)
                {
                    Character thisclient2 = ClientList[i]._client;
                    thisclient2.Shoot = data[1];
                    for (int j = 0; j < ClientList.Count; j++)
                    {
                        Socket sj = ClientList[j]._Socket;
                        if (sj != s)
                        {
                            Character otherclient = ClientList[j]._client;
                            SendData(sj, "SHOOT;" + "1");
                        }
                    }
                    thisclient2.Shoot = "";
                }
            }
        }

        public void ADVANCEWAVE_Packet(Socket s, string[] data)
        {
            Character thisclient = GetCharacter(s);
            if (thisclient != null)
            {
                //AddLog("MSG_FATAL", data[1].ToString());
                thisclient.WaveNumber = int.Parse(data[1]);
                AddLog("MSG_INFO", thisclient.Name + " is advancing wave..");
            }
            //SendToAllConnected("SPAWNM;" + data[1] + ";1;" + data[1]);
        }

        // Stats and Skills Packets
        public void LEVELUPSKILL_Packet(Socket s, string[] data)
        {
            Character thisclient = GetCharacter(s);
            bool found = false;
            for (int i = 0; i < thisclient.Skills.Count; i++)
            {
                if (thisclient.Skills[i].Id == int.Parse(data[1]))
                {
                    //thisclient.Skills[i].Level = int.Parse(data[2]) + 1;
                    thisclient.Skills[i].Level++;
                    thisclient.SkillPoints--;
                    found = true;
                    //thisclient.SaveSkills();
                    SendData(s, "LEVELUPSKILL;" + thisclient.Skills[i].Id + ";" + thisclient.Skills[i].Level + ";" + thisclient.SkillPoints);
                }
            }
            if (found)
                AddLog("MSG_INFO", thisclient.Name + " Leveled up a skill");
            else
                AddLog("MSG_INFO", thisclient.Name + " Error while leveling up a skill. Skill not found.. (" + data[1] + " " + data[2] + ")");
        }

        public void ACTIVATESKILL_Packet( Socket s, string[] data)
        {
            Character thisclient = GetCharacter(s);
            bool found = false;
            for (int i = 0; i < thisclient.Skills.Count; i++)
            {
                if (thisclient.Skills[i].Id == int.Parse(data[1]))
                {
                    thisclient.Skills[i].Active = bool.Parse(data[2]);
                    thisclient.SaveSkills();
                    SendData(s, "ACTIVATESKILL;" + thisclient.Skills[i].Id + ";" + thisclient.Skills[i].Active);
                    found = true;
                }
            }
            if (found)
                AddLog("MSG_INFO", thisclient.Name + " Set a skill to active/deactive.");
            else
                AddLog("MSG_INFO", thisclient.Name + " Error while activating/de-activating up a skill. Skill not found.. (" + data[1] + " " + data[2] + ")");
        }

        public void LEARNSKILL_Packet(Socket s, string[] data)
        {
            Character thisclient = GetCharacter(s);
            int skillid = int.Parse(data[1]);
            int slotid = int.Parse(data[2]);
            if(thisclient.LearnSkill(skillid, slotid))
            {
                thisclient.SaveSkills();
                SendData(s, "LEVELUPSKILL;" + skillid + ";1;" + thisclient.SkillPoints);
                AddLog("MSG_INFO", thisclient.Name + " Learned skill id = " + skillid);
            }
            else
                AddLog("MSG_FATAL", "Unexpected error while learning skill id = " + skillid + " slot = " + slotid);
        }

        public void STATUP_Packet(Socket s, string[] data)
        {
            Character thisclient = GetCharacter(s);
            switch (data[1])
            {
                case "1":
                    thisclient.STR++;
                    thisclient.StatPoints--;
                    thisclient.SaveStats();
                    AddLog("MSG_HACK", thisclient.Name + " Increased stat " + data[1]);
                    SendData(s, "STATSUP;" + data[1] + ";" + thisclient.STR + ";" + thisclient.StatPoints);
                    break;
                case "2":
                    thisclient.DEX++;
                    thisclient.StatPoints--;
                    thisclient.SaveStats();
                    AddLog("MSG_HACK", thisclient.Name + " Increased stat " + data[1]);
                    SendData(s, "STATSUP;" + data[1] + ";" + thisclient.DEX + ";" + thisclient.StatPoints);
                    break;
                case "3":
                    thisclient.INT++;
                    thisclient.StatPoints--;
                    thisclient.SaveStats();
                    AddLog("MSG_HACK", thisclient.Name + " Increased stat " + data[1]);
                    SendData(s, "STATSUP;" + data[1] + ";" + thisclient.INT + ";" + thisclient.StatPoints);
                    break;
                case "4":
                    thisclient.Luck++;
                    thisclient.StatPoints--;
                    thisclient.SaveStats();
                    AddLog("MSG_HACK", thisclient.Name + " Increased stat " + data[1]);
                    SendData(s, "STATSUP;" + data[1] + ";" + thisclient.Luck + ";" + thisclient.StatPoints);
                    break;
                default:
                    AddLog("MSG_HACK", "Stat ID not found.");
                    break;
            }
        }

        // inventory and items
        public void INVENTORY_SLOT_Packet(Socket s, string[] data)
        {
            Character thisclient = GetCharacter(s);
            if(int.Parse(data[3]) < 0)
            {
                AddLog("MSG_FATAL", "INVENTORY_SLOT_Packet received a slotid < 0 " + data[3]);
                return;
            }
            Item item = thisclient.EquipmentList[int.Parse(data[3])];
            if (item.ID < 0)
            {
                AddLog("MSG_FATAL", "CHANGESLOT - Unknown item in slot " + data[3]);
                return;
            }
            if(item.ID == int.Parse(data[1]))
            {
                Item tmp = item;
                item = new Item(); // might break 
                thisclient.EquipmentList[int.Parse(data[3])] = new Item();
                tmp.Slot = int.Parse(data[2]);
                thisclient.EquipmentList[int.Parse(data[2])] = tmp;
                AddLog("MSG_INFO", "Item " + tmp.ID + " was moved to slot " + data[2]);
            }
        }

        public void INVENTORY_PICKUP_Packet(Socket s, string[] data)
        {
            Character thisclient = GetCharacter(s);
            if(thisclient.EquipmentList[int.Parse(data[1])].ID < 0) // empty slot
            {
                Item item = ItemDatabase.GetItemByID(int.Parse(data[2]));
                item.Power = int.Parse(data[3]);
                item.Defense = int.Parse(data[4]);
                item.HP = int.Parse(data[5]);
                item.Stat1 = int.Parse(data[6]);
                item.Stat2 = int.Parse(data[7]);
                item.Slot = int.Parse(data[1]);
                thisclient.EquipmentList[int.Parse(data[1])] = item;

                AddLog("MSG_INFO", thisclient.Name + " Item added successfully. to slot " + data[1] + " itemid = " + item.ID + " power = " + item.Power);
                SendData(s, "PICKUPITEM;" + data[1] + ";" + item.ToPacket());
                thisclient.SaveItems();
            }
            else
            {
                AddLog("MSG_FATAL", "INVENTORY_PICKUP_Packet - adding item to an existing slot = " + data[1]);
                Item item = ItemDatabase.GetItemByID(int.Parse(data[2]));
                item.Power = int.Parse(data[3]);
                item.Defense = int.Parse(data[4]);
                item.HP = int.Parse(data[5]);
                item.Stat1 = int.Parse(data[6]);
                item.Stat2 = int.Parse(data[7]);
                int tmpslot = thisclient.GetEmptySlot();
                if (tmpslot > 0)
                {
                    item.Slot = tmpslot;
                    thisclient.EquipmentList[tmpslot] = item;
                    SendData(s, "PICKUPITEM;" + tmpslot + ";" + item.ToPacket());
                    AddLog("MSG_FATAL", "INVENTORY_PICKUP_Packet - adding item to an existing slot = " + data[1] + "Overwritten to slot = " + tmpslot + " itemid = " + item.ID + " stat1 = " + item.Stat1);
                }
                else
                    AddLog("MSG_INFO", thisclient.Name + " Error while adding an item. GetEmptySlot returned -1 " + data[1] + " itemid = " + item.ID);

            }
        }

        public void KILL_MONSTER_Packet(Socket s, string[] data)
        {
            Character thisclient = GetCharacter(s);
            AddLog("MSG_DEBUG", "IN: " + data[0] + " " + data[1] + " " + data[2] + " " + data[3]);
            int uniquemonid = int.Parse(data[1]); //unique mon id
            int monid = int.Parse(data[2]); // monster id (type)
            int mapid = int.Parse(data[3]);
            Monster m = monsters.GetMonsterByID(monid);
            bool leveledUp = false;
            if(m.MonsterID > 0)
            {
                thisclient.EXP += m.EXP;
                while(thisclient.EXP >= EXPTable.GetExpNeededForLevel(thisclient.Level))
                {
                    // send level up packet
                    thisclient.EXP = thisclient.EXP - EXPTable.GetExpNeededForLevel(thisclient.Level);
                    thisclient.Level++;
                    thisclient.StatPoints += 3;
                    thisclient.SkillPoints += 3;
                    thisclient.MaxHP += (uint) ExtraStaticFunctions.GetRandomNumber(20, 30);
                    thisclient.MaxMP += (uint)ExtraStaticFunctions.GetRandomNumber(6, 15);
                    thisclient.SaveStats(); // should remove later.
                    SendData(s, "LEVELUP;" + thisclient.Packet());
                    AddLog("MSG_INFO", thisclient.Name + " Leveled up to " + thisclient.Level);
                    leveledUp = true;
                }
                if(!leveledUp)
                    SendData(s, "GAINEXP;" + thisclient.EXP);
                AddLog("MSG_INFO", thisclient.Name + " Killed monsterid = " + monid + " and gained " + m.EXP + "exp");
                lock(Maps.MapList)
                {
                    for(int i=0; i < Maps.MapList.Count; i++)
                    {
                        for(int j=0; j < Maps.MapList[i].CharactersInMap.Count; j++)
                        {
                            Character charinmap = Maps.MapList[i].CharactersInMap[j];
                            if(charinmap.Name == thisclient.Name && charinmap.Id == thisclient.Id)
                            {
                                //AddLog("MSG_DEBUG", "Found character " + thisclient.Name + " in map");
                                for(int k=0; k < Maps.MapList[i].MonstersInMap.Count; k++)
                                {
                                    if(Maps.MapList[i].MonstersInMap[k].MonsterID == monid && Maps.MapList[i].MonstersInMap[k].ID == uniquemonid)
                                    {
                                        AddLog("MSG_DEBUG", "Found monster in map!!! YAY - now removing it.");
                                        Maps.MapList[i].MonstersInMap.RemoveAt(k);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                AddLog("MSG_FATAL", "Unknown monster found. monid = " + data[2]);
            }
        }
    }
}
*/
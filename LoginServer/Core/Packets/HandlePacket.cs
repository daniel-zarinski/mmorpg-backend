using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using LoginServer.Static;
using System.Threading;
namespace LoginServer.Core
{
    internal partial class ServerCore
    {
        private void LOGIN_Packet(Socket s, string username, string password)
        {
            int Error = 0;
            ulong userID = ExecuteLogin(username, password, Database, ref Error);
            if (userID == 0)
            {
                Console.WriteLine("MSG_WARNING", "Login attempt failed. username=" + username + " password=" + password + " and Error = " + Error);
                SendData(s, "LOGIN;0;" + Error);
                return;
            }
            else
            {
                for(int i=0; i < ClientList.Count; i++)
                {
                    if(ClientList[i]._Socket == s)
                    {
                        ClientList[i].IsActive = true;
                    }
                }
                Console.WriteLine("MSG_INFO", "Account logged in UserID = " + userID);

            }
            SendData(s, "LOGIN;" + GetAllCharactersAndInfo((int)userID, Database) + MyEncryption.CreateCharacterHash(userID.ToString()));
        }

        private void GETWORLD_Packet(Socket s, string[] data, ref ClientList _thisclient)
        {
            Character thisclient;
            thisclient = new Character(Convert.ToUInt64(data[1]), Database.MYSQL, s, Database.ItemDatabase);
            _thisclient._client = thisclient;
            if (thisclient.CharHash != data[2])
            {
                Console.WriteLine("MSG_INFO", "Hash code = " + data[2]);
                return;
            }
            Console.WriteLine("MSG_INFO", "User selected CharacterName = " + thisclient.Name);
            int index = -1;
            lock (ClientList)
            {
                for (int i = 0; i < ClientList.Count; i++)
                {
                    if (ClientList[i]._Socket.RemoteEndPoint.ToString().Equals(s.RemoteEndPoint.ToString()))
                    {
                        ClientList[i] = new ClientList(thisclient);
                        index = i;
                    }
                }
            }

            //ClientList.Add(new ClientList(thisclient));
            if (index < 0)
            {
                Console.WriteLine("MSG_FATAL", "CLIENT COULD NOT BE FOUND IN ClientList. WTF?");
            }

            if (thisclient != null && thisclient.Id > 0)
            {
                Box.AddCharacter(thisclient);
                string temp = "0;" + thisclient.Level + ";25;";
                for (int i = 0; i < thisclient.Skills.Count; i++)
                {
                    temp += thisclient.Skills[i].Id + ";" + thisclient.Skills[i].Level + ";" + thisclient.Skills[i].Active + ";";
                }
                SendData(s, "GETWORLD;" + thisclient.Packet());
                SendData(s, "SKILLS;" + temp);
                SendData(s, "INVENTORY;" + thisclient.ItemsPacket());
                //Database.MapList.Add(new MapList(thisclient, 0));
                Console.WriteLine("MSG_FATAL", thisclient.Packet());

                //SendToAllConnected("USERCONNECTED;" + thisclient.Id + ";" + thisclient.Name + ";" + thisclient.Level + ";" + thisclient.MaxHP + ";" + thisclient.MaxMP + ";" + thisclient.EquippedItemsPacket());
                Console.WriteLine("MSG_INFO", "Character information sent.");
            }
            else
                Console.WriteLine("MSG_FATAL", "Character information could not be sent. Unknown charatcer.");
            thisclient = null;
        }

        private void MOVE_Packet(Socket s, string data)
        {
            // structure: x - y- vx - vy - facingright
            Character thisclient = GetCharacter(s);
            string movepak = string.Empty;
            if (thisclient == null)
                return; // might crash
            movepak = data.Remove(0, 5);
            //Console.WriteLine(movepak);
            SendToAllConnected("RMOVE;" + thisclient.Id + ";" + movepak);
            string[] args = data.Split(';');
            float posx, posy;
            thisclient.X = (float.TryParse(args[args.Length-4], out posx)) ? float.Parse(args[args.Length-4]) : 0;
            thisclient.Y = (float.TryParse(args[args.Length-2], out posy)) ? float.Parse(args[args.Length-2]) : 0;
            //Console.WriteLine("X = " + thisclient.X + " Y = " + thisclient.Y);
        }

        private void SHOOT_Packet(Socket s, string[] data)
        {
            //ulong id = character.Id;
            //Console.WriteLine("MSG_FATAL", "ID = " + id);
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


        // Stats and Skills Packets
        private void LEVELUPSKILL_Packet(Socket s, string[] data)
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
                Console.WriteLine("MSG_INFO", thisclient.Name + " Leveled up a skill");
            else
                Console.WriteLine("MSG_INFO", thisclient.Name + " Error while leveling up a skill. Skill not found.. (" + data[1] + " " + data[2] + ")");
        }

        private void ACTIVATESKILL_Packet(Socket s, string[] data)
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
                Console.WriteLine("MSG_INFO", thisclient.Name + " Set a skill to active/deactive.");
            else
                Console.WriteLine("MSG_INFO", thisclient.Name + " Error while activating/de-activating up a skill. Skill not found.. (" + data[1] + " " + data[2] + ")");
        }

        private void LEARNSKILL_Packet(Socket s, string[] data)
        {
            Character thisclient = GetCharacter(s);
            int skillid = int.Parse(data[1]);
            int slotid = int.Parse(data[2]);
            if (thisclient.LearnSkill(skillid, slotid))
            {
                thisclient.SaveSkills();
                SendData(s, "LEVELUPSKILL;" + skillid + ";1;" + thisclient.SkillPoints);
                Console.WriteLine("MSG_INFO", thisclient.Name + " Learned skill id = " + skillid);
            }
            else
                Console.WriteLine("MSG_FATAL", "Unexpected error while learning skill id = " + skillid + " slot = " + slotid);
        }

        private void STATUP_Packet(Socket s, string[] data)
        {
            Character thisclient = GetCharacter(s);
            switch (data[1])
            {
                case "1":
                    thisclient.STR++;
                    thisclient.StatPoints--;
                    thisclient.SaveStats();
                    Console.WriteLine("MSG_HACK", thisclient.Name + " Increased stat " + data[1]);
                    SendData(s, "STATSUP;" + data[1] + ";" + thisclient.STR + ";" + thisclient.StatPoints);
                    break;
                case "2":
                    thisclient.DEX++;
                    thisclient.StatPoints--;
                    thisclient.SaveStats();
                    Console.WriteLine("MSG_HACK", thisclient.Name + " Increased stat " + data[1]);
                    SendData(s, "STATSUP;" + data[1] + ";" + thisclient.DEX + ";" + thisclient.StatPoints);
                    break;
                case "3":
                    thisclient.INT++;
                    thisclient.StatPoints--;
                    thisclient.SaveStats();
                    Console.WriteLine("MSG_HACK", thisclient.Name + " Increased stat " + data[1]);
                    SendData(s, "STATSUP;" + data[1] + ";" + thisclient.INT + ";" + thisclient.StatPoints);
                    break;
                case "4":
                    thisclient.Luck++;
                    thisclient.StatPoints--;
                    thisclient.SaveStats();
                    Console.WriteLine("MSG_HACK", thisclient.Name + " Increased stat " + data[1]);
                    SendData(s, "STATSUP;" + data[1] + ";" + thisclient.Luck + ";" + thisclient.StatPoints);
                    break;
                default:
                    Console.WriteLine("MSG_HACK", "Stat ID not found.");
                    break;
            }
        }

        // inventory and items
        private void INVENTORY_SLOT_Packet(Socket s, string[] data)
        {
            Character thisclient = GetCharacter(s);
            if (int.Parse(data[3]) < 0)
            {
                Console.WriteLine("MSG_FATAL", "INVENTORY_SLOT_Packet received a slotid < 0 " + data[3]);
                return;
            }
            Item item = thisclient.EquipmentList[int.Parse(data[3])];
            if (item.ID < 0)
            {
                Console.WriteLine("MSG_FATAL", "CHANGESLOT - Unknown item in slot " + data[3]);
                return;
            }
            if (item.ID == int.Parse(data[1]))
            {
                Item tmp = item;
                item = new Item(); // might break 
                thisclient.EquipmentList[int.Parse(data[3])] = new Item();
                tmp.Slot = int.Parse(data[2]);
                thisclient.EquipmentList[int.Parse(data[2])] = tmp;
                Console.WriteLine("MSG_INFO", "Item " + tmp.ID + " was moved to slot " + data[2]);
            }
        }

        private void INVENTORY_PICKUP_Packet(Socket s, string[] data)
        {
            Character thisclient = GetCharacter(s);
            if (thisclient.EquipmentList[int.Parse(data[1])].ID < 0) // empty slot
            {
                Item item = Database.ItemDatabase.GetItemByID(int.Parse(data[2]));
                item.Power = int.Parse(data[3]);
                item.Defense = int.Parse(data[4]);
                item.HP = int.Parse(data[5]);
                item.Stat1 = int.Parse(data[6]);
                item.Stat2 = int.Parse(data[7]);
                item.Slot = int.Parse(data[1]);
                thisclient.EquipmentList[int.Parse(data[1])] = item;

                Console.WriteLine("MSG_INFO", thisclient.Name + " Item added successfully. to slot " + data[1] + " itemid = " + item.ID + " power = " + item.Power);
                SendData(s, "PICKUPITEM;" + data[1] + ";" + item.ToPacket());
                thisclient.SaveItems();
            }
            else
            {
                Console.WriteLine("MSG_FATAL", "INVENTORY_PICKUP_Packet - adding item to an existing slot = " + data[1]);
                Item item = Database.ItemDatabase.GetItemByID(int.Parse(data[2]));
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
                    Console.WriteLine("MSG_FATAL", "INVENTORY_PICKUP_Packet - adding item to an existing slot = " + data[1] + "Overwritten to slot = " + tmpslot + " itemid = " + item.ID + " stat1 = " + item.Stat1);
                }
                else
                    Console.WriteLine("MSG_INFO", thisclient.Name + " Error while adding an item. GetEmptySlot returned -1 " + data[1] + " itemid = " + item.ID);

            }
        }

        private void INVENTORY_PICKUP_GOLD_Packet(Socket s, string[] data)
        {
            Character thisclient = GetCharacter(s);
            if (thisclient == null)
                return;
            ulong pickupgold = 0;

            if (ulong.TryParse(data[1], out pickupgold))
            {
                if (pickupgold > 0)
                {
                    thisclient.Gold += pickupgold;
                    SendData(s, "PICKUPGOLD;" + thisclient.Gold + ";" + data[1]);
                }
            }

        }

        private void KILL_MONSTER_Packet(Socket s, string[] data)
        {
            Character thisclient = GetCharacter(s);
            Console.WriteLine("MSG_DEBUG", "IN: " + data[0] + " " + data[1] + " " + data[2] + " " + data[3]);
            int uniquemonid = int.Parse(data[1]); //unique mon id
            int monid = int.Parse(data[2]); // monster id (type)
            int mapid = int.Parse(data[3]);
            Monster m = Database.monsters.GetMonsterByID(monid);
            bool leveledUp = false;
            if (m.MonsterID > 0)
            {
                thisclient.EXP += m.EXP;
                while (thisclient.EXP >= EXPTable.GetExpNeededForLevel(thisclient.Level))
                {
                    // send level up packet
                    thisclient.EXP = thisclient.EXP - EXPTable.GetExpNeededForLevel(thisclient.Level);
                    thisclient.Level++;
                    thisclient.StatPoints += 3;
                    thisclient.SkillPoints += 3;
                    thisclient.MaxHP += (uint)ExtraStaticFunctions.GetRandomNumber(20, 30);
                    thisclient.MaxMP += (uint)ExtraStaticFunctions.GetRandomNumber(6, 15);
                    thisclient.SaveStats(); // should remove later.
                    SendData(s, "LEVELUP;" + thisclient.Packet());
                    Console.WriteLine("MSG_INFO", thisclient.Name + " Leveled up to " + thisclient.Level);
                    leveledUp = true;
                }
                if (!leveledUp)
                    SendData(s, "GAINEXP;" + thisclient.EXP);
                Console.WriteLine("MSG_INFO", thisclient.Name + " Killed monsterid = " + monid + " and gained " + m.EXP + "exp");
                lock (Database.Maps.MapList)
                {
                    for (int i = 0; i < Database.Maps.MapList.Count; i++)
                    {
                        for (int j = 0; j < Database.Maps.MapList[i].CharactersInMap.Count; j++)
                        {
                            Character charinmap = Database.Maps.MapList[i].CharactersInMap[j];
                            if (charinmap.Name == thisclient.Name && charinmap.Id == thisclient.Id)
                            {
                                //Console.WriteLine("MSG_DEBUG", "Found character " + thisclient.Name + " in map");
                                for (int k = 0; k < Database.Maps.MapList[i].MonstersInMap.Count; k++)
                                {
                                    if (Database.Maps.MapList[i].MonstersInMap[k].MonsterID == monid && Database.Maps.MapList[i].MonstersInMap[k].ID == uniquemonid)
                                    {
                                        Console.WriteLine("MSG_DEBUG", "Found monster in map!!! YAY - now removing it.");
                                        Database.Maps.MapList[i].MonstersInMap.RemoveAt(k);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("MSG_FATAL", "Unknown monster found. monid = " + data[2]);
            }
        }

        private void CHAT_PLAYER(Socket s, string pak)
        {
            Character thisclient = GetCharacter(s);
            string msg = string.Empty;
            msg = "CHAT;" + thisclient.Name + ";" + pak.Substring(5, pak.Length-5);
            SendToAllConnected(msg);
        }

        private void CLIENTREQUEST_Packet(Socket s, string[] pak)
        {
            uint clientid = uint.Parse(pak[1]);
            for(int i=0; i < ClientList.Count; i++)
            {
                if(ClientList[i]._client.Id == clientid)
                {
                    SendData(s, "USERCONNECTED;" + ClientList[i]._client.ClientConnectedPacket());
                }
            }
        }

        // Wave Stuff
        private void WAVE_SELECT_Packet(Socket s, string[] pak)
        {
            int waveid;
            Character thisclient = GetCharacter(s);
            if (thisclient == null)
                return;
            if (!int.TryParse(pak[1], out waveid))
                return;
            if (thisclient.MaxWaveNumber < int.Parse(pak[1]))
                return;

            thisclient.WaveNumber = int.Parse(pak[1]);
            SendData(s, "WAVESELECT;" + thisclient.WaveNumber + ";" + Database.waves.getWaveByID(thisclient.WaveNumber).MapID + ";" + thisclient.MapID);
            //Database.Maps.CreateNewWave(thisclient, Database.waves);
            Database.Maps.AddToMap(thisclient);

            //Database.Maps.MapList[thisclient.MapID].GameStarted = true;
            // send to all that player was moved from map

        }

        private void WAVE_START_Packet(Socket s, string[] pak) // should be moved to WorldThread
        {
            int servermapid, waveid;
            bool characterfound = false;
            Character thisclient = GetCharacter(s);

            if (thisclient == null)
                return;
            if (!int.TryParse(pak[1], out servermapid))
                return;
            if (!int.TryParse(pak[2], out waveid))
                return;
            if (servermapid > Database.Maps.MapList.Count)
                return;

            Map thismap = Database.Maps.MapList[thisclient.MapID];

            for(int i=0; i < thismap.CharactersInMap.Count; i++)
            {
                if(thismap.CharactersInMap[i].Id == thisclient.Id)
                {
                    characterfound = true;
                }
            }
            if (!characterfound)
                return;
            thismap.WaveNumber = waveid;
            thismap.GameStarted = true;
            Console.WriteLine("WAVE STRATING on map " + thisclient.MapID);

        }

        private void NPC_BuyItem(Socket s, string[] pak, Character thisclient)
        {
            // expected
            int npcid = int.TryParse(pak[1], out npcid) ? npcid : 0;// npcID - int
            int itemid = int.TryParse(pak[2], out itemid) ? itemid : 0;// itemID - int
            int cost = int.TryParse(pak[3], out cost) ? cost : 0;// cost - int
            bool found = false;
            if (thisclient == null || thisclient.Id <= 0) return;

            NPC thisnpc = Database.Instance.NPCsDatabase.GetNPCByID(npcid);
            Item item = new Item();

            if (thisnpc.ID == 0) return;
            Console.WriteLine("MSG_INFO", "Player buying itemid = " + itemid + " from " + thisnpc.Name + " for " + cost);
            for(int i=0; i < thisnpc.SellList.Length; i++)
            {
                if(thisnpc.SellList[i].Item.ID == itemid && thisnpc.SellList[i].Cost == cost)
                {
                    item = Database.Instance.ItemDatabase.GetItemByID(thisnpc.SellList[i].Item.ID);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Console.WriteLine("Item could not be found.");
                return;
            }

            if(thisclient.Gold < (ulong)cost)
            {
                Console.WriteLine("Insufficient gold (need: " + cost + " have: " + thisclient.Gold + ")");
                return;
            }
            
            int tmpslot = thisclient.GetEmptySlot();
            if (tmpslot >= 0)
            {
                thisclient.Gold -= (ulong)cost;
                item.Slot = tmpslot;
                thisclient.EquipmentList[tmpslot] = item;
                SendData(s, "BUYITEM;" + tmpslot + ";" + thisclient.Gold + ";" + item.ToPacket());
                Console.WriteLine("MSG_INFO", "Bought item successfully! ");
            }
            else
            {
                Console.WriteLine("no slots");
            }

        }
    }
}
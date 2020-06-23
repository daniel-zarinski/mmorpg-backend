using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LoginServer.Core;

namespace LoginServer.Threads
{
    class WorldThread
    {
        private Thread WorldProcess;


        public WorldThread(Database db, ServerCore sc)
        {
            object args = new object[2] { db, sc };

            WorldProcess = new Thread(new ParameterizedThreadStart(MapProcess));
            WorldProcess.Priority = ThreadPriority.BelowNormal;
            WorldProcess.IsBackground = true;
            WorldProcess.Start(args);
        }

        private void MapProcess(object args)
        {
            Array _argArray = new object[2];
            _argArray = (Array)args;
            Database db = (Database)_argArray.GetValue(0);
            ServerCore sc = (ServerCore)_argArray.GetValue(1);
            while(ServerCore.ServerRunning)
            {
                for (int i = 0; i < db.Maps.MapList.Count; i++)
                {
                    if (db.Maps.MapList[i].CharactersInMap.Count < 1)
                    {
                        lock (db.Maps.MapList)
                        {
                            db.Maps.MapList.RemoveAt(i);
                        }
                        continue; // skip empty maps
                    }
                    Map thismap = db.Maps.MapList[i];
                    /*if (!IsConnected(Maps.MapList[i].CharactersInMap[0].socketClient))
                    {
                        AddLog("MSG_INFO", "Detected disconnected character..");
                        continue;
                    }*/

                    if (thismap.GameStarted)
                    {
                        if (!db.Maps.MapList[i].Active)
                        {
                            db.Maps.CreateNewWave(thismap.WaveNumber, db.waves);
                            string monpak = "SPAWNM;";
                            for (int m = 0; m < db.Maps.MapList[i].MonstersInMap.Count; m++)
                            {
                                Monster mon = db.Maps.MapList[i].MonstersInMap[m];
                                db.Maps.MapList[i].MonstersInMap[m].ID = m;
                                db.Maps.MapList[i].MonstersInMap[m].MapID = i;
                                // monsterid | MapID | uniqueID | | MONEY | dropIDs | X | Y
                                string t = mon.GetDropPack(db.ItemDatabase);

                                monpak += mon.MonsterID.ToString() + "|" + i + "|" + m + "|" + mon.GetMoney() + "|" + t + "|" + mon.position.XCoord + "|" + mon.position.YCoord + ";";
                            }

                            Console.WriteLine("MSG_INFO", "Character found in map : " + i + " name = " + db.Maps.MapList[i].CharactersInMap[0].Name + "Packet sent = " + monpak);
                            sc.SendData(db.Maps.MapList[i].CharactersInMap[0].socketClient, monpak);
                            db.Maps.MapList[i].Active = true;
                        }
                    }
                    //sc.SendData(db.Maps.MapList[i].CharactersInMap[0].socketClient, "TEST;" + packetsSent);
                    //db.Maps.MapList[i].CharactersInMap[0].STR = uint.Parse(packetsSent.ToString());
                    //packetsSent++;
                }

                Thread.Sleep(2000);
            }

            Thread.Sleep(1);
        }
    }
}

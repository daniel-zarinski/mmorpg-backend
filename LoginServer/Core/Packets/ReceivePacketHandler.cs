using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace LoginServer.Core
{
    internal partial class ServerCore
    {
        public void HandlePak(object t, ref ClientList _thisclient)
        {
            Array _argArray = new object[2];
            _argArray = (Array)t;
            ClientList thisclientlist = (ClientList)_argArray.GetValue(0);
            Socket s = thisclientlist._Socket;
            Character thisclient = thisclientlist._client;
            string textReceived = (string)_argArray.GetValue(1);

            string[] data = textReceived.Split(';');

            if(data[0] != "MOVE")
                Console.WriteLine("MSG_INFO", "IN: " + textReceived + " id = " + thisclient.Id);

            switch (data[0])
            {
                case "LOGIN": LOGIN_Packet(s, data[1], data[2]); break;

                case "GETWORLD": GETWORLD_Packet(s, data, ref _thisclient); break;

                case "MOVE": MOVE_Packet(s, textReceived); break;

                case "SHOOT": SHOOT_Packet(s, data); break;

                case "LEVELUPSKILL": LEVELUPSKILL_Packet(s, data); break;

                case "ACTIVATESKILL": ACTIVATESKILL_Packet(s, data); break;

                case "LEARNSKILL": LEARNSKILL_Packet(s, data); break;

                // stats
                case "STATSUP": STATUP_Packet(s, data); break;

                // inventory
                case "CHANGESLOT": INVENTORY_SLOT_Packet(s, data); break;

                case "PICKUPITEM": INVENTORY_PICKUP_Packet(s, data); break;

                case "PICKUPGOLD": INVENTORY_PICKUP_GOLD_Packet(s, data); break;

                //Kill Monsters

                case "KILLMONSTER": KILL_MONSTER_Packet(s, data); break;

                // Chat Stuff

                case "CHAT": CHAT_PLAYER(s, textReceived); break;

                // Client Requests
                case "REQUESTCLIENT": CLIENTREQUEST_Packet(s, data); break;


                // Wave Stuff
                case "SELECTLEVEL": WAVE_SELECT_Packet(s, data); break;

                case "WAVESTART": WAVE_START_Packet(s, data); break;

                case "BUYITEM": NPC_BuyItem(s, data, thisclient); break;

                default: Console.WriteLine("MSG_FATAL", "Unknown packet received:" + data[0]); break;
            }
        }
    }
}

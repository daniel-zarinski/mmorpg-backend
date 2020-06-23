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
        private void HandlePacket(object t)
        {
            Array _argArray = new object[2];
            _argArray = (Array)t;
            Socket s = (Socket)_argArray.GetValue(0);
            string textReceived = (string)_argArray.GetValue(1);
            Command response = new Command();

            //LogtoText("RECEIVING PACKET: " + textReceived);
            string[] data2 = textReceived.Split('/');
            foreach (string _data in data2)
            {
                string[] data = _data.Split(';');
                if (data[0].Length == 0)
                    continue;
                //if(data[0] != "RMOVE")
                //    AddLog("MSG_INFO", "Receive: " + data[0]);
                switch (data[0])
                {
                    case "LOGIN": LOGIN_Packet(s, data[1], data[2]); break;

                    case "GETWORLD": GETWORLD_Packet(s, data); break;

                    case "MOVE": MOVE_Packet(s, data); break;

                    case "SHOOT": SHOOT_Packet(s, data);  break;

                    case "ADVANCEWAVE": ADVANCEWAVE_Packet(s, data); break;

                    case "LEVELUPSKILL": LEVELUPSKILL_Packet(s, data); break;

                    case "ACTIVATESKILL": ACTIVATESKILL_Packet(s, data); break;

                    case "LEARNSKILL": LEARNSKILL_Packet(s, data); break;

                    // stats
                    case "STATSUP":  STATUP_Packet(s, data);  break;

                    // inventory
                    case "CHANGESLOT": INVENTORY_SLOT_Packet(s, data); break;

                    case "PICKUPITEM": INVENTORY_PICKUP_Packet(s, data); break;

                    //Kill Monsters

                    case "KILLMONSTER": KILL_MONSTER_Packet(s, data); break;

                    default: AddLog("MSG_FATAL", "Unknown packet received:" + data[0]); break;
                }
            }
        }
    }
}
*/
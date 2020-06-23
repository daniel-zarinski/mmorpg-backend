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
        public void RemoveClient(Socket s)
        {
            lock (ClientList)
            {
                for (int i = 0; i < ClientList.Count; i++)
                {
                    if (ClientList[i]._Socket.RemoteEndPoint.ToString().Equals(s.RemoteEndPoint.ToString()))
                    {
                        Console.WriteLine("MSG_FATAL", "Forcing a ClientList to disconnect..");
                        try
                        {
                            Box.RemovecharacterFromBox(ClientList[i]._client);
                            Box.RemoveIPFromBox(ClientList[i]._Socket);
                            if (ClientList[i].IsActive && ClientList[i]._client.Id > 0)
                            {
                                RemoveFromMap(ClientList[i]._client);
                                ClientList[i]._client.SaveItems(); // saving items
                                ClientList[i]._client.SaveSkills(); // saving skills
                                ClientList[i]._client.SaveStats(); // saving stats
                            }
                            ClientList.RemoveAt(i);
                            Box.UpdateClientsConnetedLabel(ClientList);
                            s.Shutdown(SocketShutdown.Both);
                            s.Close();
                            s.Dispose();
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine("MSG_FATAL", "Exception thrown - IsConnected line 119 \n\n" + e.ToString());
                        }
                    }
                    continue;
                }
            }
        }

        private void RemoveFromMap(Character thisclient)
        {
            lock (Database.Maps.MapList)
            {
                for (int i = 0; i < Database.Maps.MapList.Count; i++)
                {
                    for (int k = 0; k < Database.Maps.MapList[i].CharactersInMap.Count; k++)
                    {
                        if (Database.Maps.MapList[i].CharactersInMap[k] == thisclient)
                            Database.Maps.MapList[i].CharactersInMap.RemoveAt(k);
                    }
                }
            }
        }

        public void SendToAllConnected(string msg)
        {
            for(int i=0; i < ClientList.Count; i++)
            {
                Character otherclient = ClientList[i]._client;
                if(otherclient.Id > 0 && ClientList[i].IsActive)
                {
                    SendData(otherclient.socketClient, msg);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LoginServer.Core;

namespace LoginServer.Threads
{
    class ServerThread
    {
        private Thread _serverThread;

        public ServerThread(Database db, ServerCore sc)
        {
            object args = new object[2] { db, sc };

            _serverThread = new Thread(new ParameterizedThreadStart(ServerProcess));
            _serverThread.Priority = ThreadPriority.BelowNormal;
            _serverThread.IsBackground = true;
            _serverThread.Start(args);
        }

        private void ServerProcess(object args)
        {
            Array _argArray = new object[2];
            _argArray = (Array)args;
            Database db = (Database)_argArray.GetValue(0);
            ServerCore sc = (ServerCore)_argArray.GetValue(1);

            while (ServerCore.ServerRunning)
            {
                for (int i = 0; i < sc.ClientList.Count; i++)
                {
                    if (!sc.IsConnected(sc.ClientList[i]._Socket))
                    {
                        Console.WriteLine("MSG_INFO", "Forcing a client to logout. - Login Server");
                        sc.RemoveClient(sc.ClientList[i]._Socket);
                        continue;
                    }
                    if(!sc.ClientList[i].IsActive)
                    {
                        Console.WriteLine("MSG_FATAL", "Client detected that is NOT active. - Removing");
                        sc.RemoveClient(sc.ClientList[i]._Socket);
                        continue;
                    }
                    /*Character thisclient = sc.ClientList[i]._client;
                    if (thisclient.LastSave + 60 < (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)
                    {
                        Console.WriteLine("MSG_FATAL", "SAVING!!!!!! + " + thisclient.Name);
                        thisclient.LastSave = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    }*/

                }

                Thread.Sleep(5000);
            }

            Thread.Sleep(10);
        }
    }
}

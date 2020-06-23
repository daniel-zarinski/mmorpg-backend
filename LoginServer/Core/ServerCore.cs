using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using LoginServer.Static;
using LoginServer.Threads;

namespace LoginServer.Core
{
    internal partial class ServerCore
    {
        private Socket ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private TcpListener server;
        private byte[] _buffer = new byte[1024];
        private Thread listenerThread;

        private readonly int ServerPort;
        public ClientsBox Box;

        public List<ClientList> ClientList;
        public Database Database;

        public static bool ServerRunning = false;

        public ServerCore(int port, ClientsBox clbox)
        {
            ServerPort = port;
            ClientList = new List<ClientList>();
            Box = clbox;
            Database = Database.Instance;
        }

        public void StartServer()
        {
            /*ServerSocket.Bind(new IPEndPoint(IPAddress.Any, ServerPort));
            ServerSocket.Listen(0);
            ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);*/
            ServerRunning = true;
            server = new TcpListener(IPAddress.Any, ServerPort);
            listenerThread = new Thread(Listen);
            listenerThread.Priority = ThreadPriority.Normal;
            listenerThread.IsBackground = true;
            listenerThread.Start();
            LaunchDatabaseUpdate();
            // Start up threads
            WorldThread worldThread = new WorldThread(Database.Instance, this);
            ServerThread serverThread = new ServerThread(Database.Instance, this);
        }

        private void Listen()
        {
            while(ServerRunning)
            {
                try
                {
                    server.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                try
                {
                    if(server.Pending())
                    {
                        Thread clientTh = new Thread(ClientService);
                        clientTh.Priority = ThreadPriority.BelowNormal;
                        clientTh.IsBackground = true;
                        clientTh.Start(server.AcceptSocket());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());

                }
                
                Thread.Sleep(1);
            }
            Thread.Sleep(1);
            server.Stop();
        }

        private void ClientService(object s)
        {
            Socket socket = s as Socket;
            ClientList thisclient = new ClientList(socket);
            ClientList.Add(thisclient);
            Box.AddIP(socket);
            Box.UpdateClientsConnetedLabel(ClientList);
            while (socket.Connected && IsConnected(socket))
            {
                int receiveBytes = 0;
                try
                {
                    receiveBytes = socket.Receive(_buffer);
                }
                catch
                {
                    Console.WriteLine("MSG_FATAL", "Client disconnected. - ServerCore(101) - while socket.Connected");
                    RemoveClient(socket);
                    return;
                }
                if (receiveBytes != 0)
                {
                    byte[] dataBuffer = new byte[receiveBytes];
                    Array.Copy(_buffer, dataBuffer, receiveBytes);
                    string textReceived = Encoding.ASCII.GetString(dataBuffer);

                    object _args = new object[2] { thisclient, textReceived };
                    HandlePak(_args, ref thisclient);
                }

                Thread.Sleep(1);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket s = ServerSocket.EndAccept(ar);
            ClientList.Add(new ClientList(s));
            Box.AddIP(s);
            Box.UpdateClientsConnetedLabel(ClientList);
            s.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), s);
            //ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            if (s.Connected)
            {
                int receiveBytes = 0;
                try
                {
                    receiveBytes = s.EndReceive(ar);
                }
                catch
                {
                    Console.WriteLine("MSG_FATAL", "Client disconnected. - ServerInit line 45");
                    RemoveClient(s);
                    return;
                }
                if (receiveBytes != 0)
                {
                    byte[] dataBuffer = new byte[receiveBytes];
                    Array.Copy(_buffer, dataBuffer, receiveBytes);
                    string textReceived = Encoding.ASCII.GetString(dataBuffer);

                    object _args = new object[2] { s, textReceived };
                    /*_pakThread = new Thread(new ParameterizedThreadStart(HandlePak));
                    _pakThread.Priority = ThreadPriority.BelowNormal;
                    _pakThread.IsBackground = true;
                    _pakThread.Start(_args);*/
                }
            }
            if (s.Connected)
                s.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), s);
        }

        /*public void SendData(Socket s, string msgToSend)
        {
            byte[] data = Encoding.ASCII.GetBytes(msgToSend + ";/");
            if (IsConnected(s))
            {
                s.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), s);
                ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }
        }*/

        public void SendData(Socket s, string msgToSend)
        {
            byte[] data = Encoding.ASCII.GetBytes(msgToSend + ";/");
            if (IsConnected(s))
            {
                s.Send(data);
            }

        }

        private void SendCallback(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            s.EndSend(ar);
        }

        public bool IsConnected(Socket socket) // needs to be true
        {
            try
            {
                if (!socket.Connected)
                    return false;

                if ((socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0) == true)
                {
                    return false;
                }
                else
                    return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

/*8
namespace LoginServer
{
    public partial class frmLoginServer
    {
        private void AcceptCallback(IAsyncResult ar)
        {
            Socket s = _serverSocket.EndAccept(ar);
            ClientList.Add(new ClientList(s));
            lbClients.Items.Add(s.RemoteEndPoint.ToString());
            lClientsConnected.Text = "Clients connected: " + ClientList.Count.ToString();
            s.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), s);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        public void SendData(Socket s, string msgToSend)
        {
            byte[] data = Encoding.ASCII.GetBytes(msgToSend + ";/");
            if (IsConnected(s))
            {
                s.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), s);
                _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }
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
                    AddLog("MSG_FATAL", "Client disconnected. - ServerInit line 45");
                    RemoveClient(s);
                    return;
                }
                if (receiveBytes != 0)
                {
                    byte[] dataBuffer = new byte[receiveBytes];
                    Array.Copy(_buffer, dataBuffer, receiveBytes);
                    string textReceived = Encoding.ASCII.GetString(dataBuffer);
                    // start up listening thread for packets.
                    object _args = new object[2] { s, textReceived };
                    _pakThread = new Thread(new ParameterizedThreadStart(HandlePacket));
                    _pakThread.Priority = ThreadPriority.BelowNormal;
                    _pakThread.IsBackground = true;
                    _pakThread.Start(_args);
                }
                /*else
                {
                    for (int i = 0; i < _clientSockets.Count; i++)
                    {
                        if (_clientSockets[i]._socket.RemoteEndPoint.ToString() == s.RemoteEndPoint.ToString())
                        {
                            lock (_clientSockets)
                            {
                                _clientSockets.RemoveAt(i);
                            }
                        }
                    }
                }*/
            /*}
            if(s.Connected)
                s.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), s);
        }


        private void SendCallback(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            s.EndSend(ar);
        }

        private bool IsConnected(Socket socket) // needs to be true
        {
            try
            {
                if (!socket.Connected)
                    return false;

                if ((socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0) == true)
                {
                    RemoveClient(socket);
                    lock (ClientList)
                    {
                        for (int i = 0; i < ClientList.Count; i++)
                        {
                            if (ClientList[i]._Socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                            {
                                AddLog("MSG_FATAL", "Forcing a clientSocket to disconnect.");
                                try
                                {
                                    lbClients.Items.RemoveAt(lbClients.Items.IndexOf(socket.RemoteEndPoint.ToString()));

                                }
                                catch (Exception e)
                                {
                                    AddLog("MSG_FATAL", "Exception thrown - IsConnected line 136 \n\n" + e.ToString());
                                }

                                ClientList.RemoveAt(i);
                                
                                // lClientsConnected.Text = "Clients connected: " + _clientSockets.Count.ToString();
                            }
                        }
                    }
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

        public void RemoveClient(Socket socket)
        {
            lock (ClientList)
            {
                for (int i = 0; i < ClientList.Count; i++)
                {
                    if (ClientList[i]._Socket.RemoteEndPoint.ToString().Equals(socket.RemoteEndPoint.ToString()))
                    {
                        AddLog("MSG_FATAL", "Forcing a ClientList to disconnect..");
                        try
                        {
                            lbCharacters.Items.RemoveAt(lbCharacters.Items.IndexOf(ClientList[i]._client.Name));
                            lbClients.Items.RemoveAt(lbClients.Items.IndexOf(ClientList[i]._Socket.RemoteEndPoint.ToString()));
                            RemoveFromMap(ClientList[i]._client);
                            ClientList[i]._client.SaveItems(); // saving items
                            ClientList[i]._client.SaveSkills(); // saving skills
                            ClientList[i]._client.SaveStats(); // saving stats
                            ClientList.RemoveAt(i);
                            lClientsConnected.Text = "Clients connected: " + ClientList.Count.ToString();
                            socket.Close(); // might crash loooool
                        }

                        catch (Exception e)
                        {
                            AddLog("MSG_FATAL", "Exception thrown - IsConnected line 119 \n\n" + e.ToString());
                        }
                    }
                    continue;
                }
            }
        }
    }
}
*/
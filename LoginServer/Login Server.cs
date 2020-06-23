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
using Commands;
using System.Security.Cryptography;
using LoginServer.Core;
using LoginServer.Threads;
using LoginServer.ApplicationCore;

namespace LoginServer
{
    public partial class frmLoginServer : Form
    {

        public MySQLHandler MYSQL;
        public static bool serverRunning;
        private delegate void SetString(string Type, string str);
        System.Windows.Forms.Timer timer;


        // New ServerCore
        internal ServerCore Server;

        public frmLoginServer()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            if (!Directory.Exists("log"))
                Directory.CreateDirectory("log");
        }

        private void frmLoginServer_Load(object sender, EventArgs e)
        {
            Console.SetOut(new TextBoxWriter(txtLog));
            ClientsBox ClientsBox = new ClientsBox(lbClients, lbCharacters, lClientsConnected);
            Server = new ServerCore(32211, ClientsBox);
            SetupServer();
        }

        private void SetupServer()
        {
            serverRunning = false;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(DisplayServerTime);
            timer.Start();
            Console.WriteLine("MSG_STARTUP", "Setting up server...");

            Server.StartServer();
            
            txtLog.Enabled = true;
            lStatus.Text = "Server is running...";
            lStatus.BackColor = Color.Green;
            tLog.Enabled = true;


            serverRunning = true;

            lStatus.Text = "Server is running on port: 32211";
            Console.WriteLine("MSG_STARTUP", "Server is running on port 32211");
        }

        private void bSendToSelected_Click(object sender, EventArgs e)
        {
            //ForceKickClient(tbInput.Text);
            tbLog.AppendText("Send to selected: " + tbInput.Text + "\n");
        }

        /*private void ForceKickClient(string msgToSend)
        {
            for (int i = 0; i < lbClients.SelectedItems.Count; i++)
            {
                for (int j = 0; j < ClientList.Count; j++)
                {
                    if (lbClients.SelectedItems[i].ToString().Equals(ClientList[j]._Socket.RemoteEndPoint.ToString()))
                    {
                        //SendData(_clientSockets[j]._socket, msgToSend);
                        //RemoveClient(ClientList[j]._Socket);
                        //ClientList[j]._Socket.Close();
                    }
                }
            }
        }*/

        private void bBroadcast_Click(object sender, EventArgs e)
        {
            //Broadcast(tbInput.Text);
            tbLog.AppendText("Send to all: " + tbInput.Text + "\n");
        }

        private void tLog_Tick(object sender, EventArgs e)
        {
            //LogToFile();
        }

        public void Log(string Type, string Msg)
        {
            switch (Type)
            {
                case "MSG_STARTUP":
                    txtLog.SelectionColor = Color.ForestGreen;
                    txtLog.AppendText("["+GetServerDateTime() + "] " + Msg + "\n");
                    break;
                case "MSG_FATAL":
                    txtLog.SelectionColor = Color.Red;
                    txtLog.AppendText("[" + GetServerDateTime() + "] " + Msg + "\n");
                    break;
                case "MSG_WARNING":
                    txtLog.SelectionColor = Color.Orange;
                    txtLog.AppendText("[" + GetServerDateTime() + "] " + Msg + "\n");
                    break;
                case "MSG_INFO":
                    txtLog.SelectionColor = Color.Blue;
                    txtLog.AppendText("[" + GetServerDateTime() + "] " + Msg + "\n");
                    break;
                case "MSG_LOAD":
                    txtLog.SelectionColor = Color.DarkOrange;
                    txtLog.AppendText("[" + GetServerDateTime() + "] ");
                    txtLog.SelectionFont = new Font(txtLog.Font, FontStyle.Bold);
                    txtLog.AppendText(Msg + "\n");
                    break;
                case "MSG_DEBUG":
                    txtLog.SelectionColor = Color.ForestGreen;
                    txtLog.AppendText("[" + GetServerDateTime() + "] " + Msg + "\n");
                    break;
                default:
                    txtLog.SelectionColor = Color.Black;
                    txtLog.AppendText("[" + GetServerDateTime() + "] " + Msg + "\n");
                    break;
            }
            LogtoText(Msg);
        }
        private void LogtoText(string Msg)
        {
            string file = ".\\log\\" + DateTime.Now.ToString("yyyy-M-d") + ".txt";
            if (!File.Exists(file))
            {
                File.Create(file).Dispose();
                using (TextWriter tw = new StreamWriter(file))
                {
                    tw.WriteLine("This is the log file of the " + DateTime.Now.ToString("yyyy-M-d"));
                    tw.Close();
                }
            }
            using (TextWriter tw = new StreamWriter(file, true))
            {
                tw.WriteLine("[" + GetServerDateTime() + "] " + Msg);
                tw.Close();
            }
        }
        public void AddLog(string Type, string str)
        {
            Invoke
            (
                new SetString(Log),
                new Object[] { Type, str }
            );
        }

        private string GetServerDateTime()
        {
            DateTime date = DateTime.Now;

            return String.Format("{0: yyyy'-'MM'-'dd HH:mm:ss}", date);
        }

        private void DisplayServerTime(object sender, EventArgs e)
        {
            lbTime.Text = GetServerDateTime();
        }

        private void itemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = ".\\Includes\\Items.json";
            ImportJson imp;
            if (!File.Exists(file))
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (!openFileDialog1.FileName.Contains("Items"))
                    {
                        MessageBox.Show("Incorrect file to import..");
                        return;
                    }
                    System.IO.StreamReader sr = new
                       System.IO.StreamReader(openFileDialog1.FileName);
                    string t = sr.ReadToEnd();
                    sr.Close();
                    imp = new ImportJson(t, Server.Database);
                    imp.ImportItems();
                }
            }
            else
            {
                StreamReader reader = new StreamReader(file);
                string t = reader.ReadToEnd();
                imp = new ImportJson(t, Server.Database);
                imp.ImportItems();
            }
        }
        

        private void skillsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = ".\\Includes\\Skills.json";
            ImportJson imp;
            if (!File.Exists(file))
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (!openFileDialog1.FileName.Contains("Skills"))
                    {
                        MessageBox.Show("Incorrect file to import..");
                        return;
                    }
                    System.IO.StreamReader sr = new
                   System.IO.StreamReader(openFileDialog1.FileName);
                    string t = sr.ReadToEnd();
                    sr.Close();
                    imp = new ImportJson(t, Server.Database);
                    imp.ImportSkills();
                }
            }
            else
            {
                StreamReader reader = new StreamReader(file);
                string t = reader.ReadToEnd();
                imp = new ImportJson(t, Server.Database);
                imp.ImportSkills();
            }
        }

        private void nPCsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = ".\\Includes\\NPCs.json";
            ImportJson imp;
            if (!File.Exists(file))
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (!openFileDialog1.FileName.Contains("NPCs"))
                    {
                        MessageBox.Show("Incorrect file to import..");
                        return;
                    }
                    System.IO.StreamReader sr = new
                   System.IO.StreamReader(openFileDialog1.FileName);
                    string t = sr.ReadToEnd();
                    sr.Close();
                    imp = new ImportJson(t, Server.Database);
                    imp.ImportNPCs();
                }
            }
            else
            {
                StreamReader reader = new StreamReader(file);
                string t = reader.ReadToEnd();
                imp = new ImportJson(t, Server.Database);
                imp.ImportNPCs();
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }
    }

    public class ClientList
    {
        public Character _client { get; set; } = new Character();
        public Socket _Socket { get; set; }
        public int UserID { get; set; }
        public bool IsActive { get; set; }

        public Thread clientThread;

        public string guID;

        public ClientList(Character s)
        {
            this._client = s;
            this._Socket = s.socketClient;
            this.IsActive = true;
            this.UserID = s.AccountID;
            guID = Guid.NewGuid().ToString();
        }
        public ClientList(Socket s)
        {
            this._client = new Character(s);
            this._Socket = s;
            this.UserID = -1;
            this.IsActive = false;
            guID = Guid.NewGuid().ToString();
            //clientThread = new Thread(ServerCore.ClientDataIn);
            //clientThread.Start();
        }

        ~ClientList()
        {
            this._client = null;
            this._Socket = null;
            this.UserID = -1;
        }
    }

}

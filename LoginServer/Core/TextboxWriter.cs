using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Net.Sockets;
using System.Collections.Generic;
namespace LoginServer.Core
{
    internal class TextBoxWriter : TextWriter
    {
        private readonly RichTextBox m_mTextBox;

        public TextBoxWriter(RichTextBox textbox)
        {
            m_mTextBox = textbox;
        }

        public override Encoding Encoding => Encoding.Default;

        public override void Write(string value)
        {
            m_mTextBox.BeginInvoke(
                new Action(() => { m_mTextBox.AppendText($"[{DateTime.Now.ToString("yyyy'-'MM'-'dd HH:mm:ss")}] {value}"); }));
        }

        public override void WriteLine(string value)
        {
            m_mTextBox.BeginInvoke(
                new Action(
                    () =>
                    {
                        m_mTextBox.AppendText($"[{DateTime.Now.ToString("yyyy'-'MM'-'dd HH:mm:ss")}] {value}{Environment.NewLine}");
                    }));
        }

        public override void WriteLine(string type, object value)
        {
            m_mTextBox.BeginInvoke(
                new Action(
                    () =>
                    {
                        m_mTextBox.SelectionColor = txtColor(type);
                        m_mTextBox.AppendText($"[{DateTime.Now.ToString("yyyy'-'MM'-'dd HH:mm:ss")}] {value}{Environment.NewLine}");
                    }));
            SaveToFile(value);
        }

        private Color txtColor(string Type)
        {
            switch (Type)
            {
                case "MSG_STARTUP":
                    return Color.ForestGreen;
                case "MSG_FATAL":
                    return Color.Red;
                case "MSG_WARNING":
                    return Color.Orange;
                case "MSG_INFO":
                    return Color.Blue;
                case "MSG_LOAD":
                    return Color.DarkOrange;
                case "MSG_DEBUG":
                    return Color.ForestGreen;
                default:
                    return Color.Black;
            }
        }
        
        private void SaveToFile(object value)
        {
            string file = ".\\log\\" + DateTime.Now.ToString("yyyy-M-d") + ".txt";
            if (!File.Exists(file))
            {
                File.Create(file).Dispose();
                using (TextWriter tw = new StreamWriter(file))
                {
                    tw.WriteLine($"[{DateTime.Now.ToString("yyyy'-'MM'-'dd HH:mm:ss")}]");
                    tw.Close();
                }
            }
            using (TextWriter tw = new StreamWriter(file, true))
            {
                tw.WriteLine($"[{DateTime.Now.ToString("yyyy'-'MM'-'dd HH:mm:ss")}] {value}");
                tw.Close();
            }
        }
        
    }

    internal class ClientsBox : ListBox
    {
        private readonly ListBox lbClients;
        private readonly ListBox lbCharacters;
        private readonly Label lClientsConnected;
        public ClientsBox(ListBox lbclients, ListBox lbcharacters, Label lclientsconnected)
        {
            lbCharacters = lbcharacters;
            lbClients = lbclients;
            lClientsConnected = lclientsconnected;
        }

        public void AddCharacter(Character s)
        {
            if (s != null)
            {
                if (s.Id > 0)
                {
                    lbCharacters.BeginInvoke(
                        new Action(
                            () =>
                            {
                                lbCharacters.Items.Add(s.Name);
                            }));
                }
                else
                    lbCharacters.Items.Add("NULL ID");
            }
            else
                lbCharacters.Items.Add("NULL Client");
        }
        public void RemovecharacterFromBox(Character s)
        {
            if(s.Id > 0 && s != null)
            lbCharacters.Items.RemoveAt(lbCharacters.Items.IndexOf(s.Name));
        }
        public void AddIP(Socket s)
        {
            if(s != null)
            {
                lbClients.BeginInvoke(
                    new Action(
                        () =>
                        {
                            lbClients.Items.Add(s.RemoteEndPoint.ToString());
                        }));
            }
        }

        public void RemoveIPFromBox(Socket s)
        {
            if (s != null)
            {
                if (lbClients.Items.Count > 0)
                {
                    lbClients.BeginInvoke(
                        new Action(
                            () =>
                            {
                                lbClients.Items.RemoveAt(lbClients.Items.IndexOf(s.RemoteEndPoint.ToString()));
                            }));
                }
                else
                    lbClients.Items.Clear();
                
            }
        }

        public void UpdateClientsConnetedLabel(List<ClientList> clientlist)
        {
            lClientsConnected.Text = "Clients connected: " + clientlist.Count.ToString();
        }

    }

}

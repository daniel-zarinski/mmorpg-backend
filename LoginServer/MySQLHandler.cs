using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace LoginServer
{
    public class MySQLHandler
    {
        private Properties.Settings settings = Properties.Settings.Default;

        private string mysqlLogin;
        private string mysqlPass;
        private string mysqlBase;
        private string mysqlHost;
        private string mysqlPort;

        private string connectionString;

        private MySqlConnection connection;

        public static object LockSQL = new object();
        
        public string MySqlLogin
        {
            set
            {
                mysqlLogin = value;
            }
        }
        public string MySqlPassword
        {
            set
            {
                mysqlPass = value;
            }
        }
        public string MySqlBase
        {
            get
            {
                return mysqlBase;
            }
            set
            {
                mysqlBase = value;
            }
        }
        public string MySqlHost
        {
            get
            {
                return mysqlHost;
            }
            set
            {
                mysqlHost = value;
            }
        }
        public string MySqlPort
        {
            set
            {
                mysqlPort = value;
            }
        }



        public MySQLHandler()
        {
            ConnectionStringFromSettings();
            connection = new MySqlConnection(connectionString);
        }

        ~MySQLHandler()
        {
            connection.Close();
        }

        public MySqlConnection Connection
        {
            get
            {
                return connection;
            }
        }

        public string CreateConnectionString(string host, string user, string password, string dataBase)
        {
            string connectionString = "server=" + host + ";port=3306;user id=" + user + "; pwd=" + password + ";database=" + dataBase + ";";
            return connectionString;
        }

        private void ConnectionStringFromSettings()
        {
            mysqlLogin = System.Configuration.ConfigurationManager.AppSettings["mysqlUsername"].ToString();
            mysqlPass = System.Configuration.ConfigurationManager.AppSettings["mysqlPassword"].ToString();
            mysqlBase = System.Configuration.ConfigurationManager.AppSettings["mysqlDatabase"].ToString();
            mysqlHost = System.Configuration.ConfigurationManager.AppSettings["mysqlServer"].ToString();

            connectionString = CreateConnectionString(mysqlHost, mysqlLogin, mysqlPass, mysqlBase);
            
        }

        public void RefreshConnection()
        {
            ConnectionStringFromSettings();
            connection = new MySqlConnection(connectionString);
        }

        public string QStore( string[] Format)
        {
            return Format.ToString();
        }
    }
}
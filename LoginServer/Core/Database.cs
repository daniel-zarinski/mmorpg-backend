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
using LoginServer;

namespace LoginServer.Core
{
    class Database
    {
        public MySQLHandler MYSQL;
        public maps Maps;
        public Skills skills;
        public Monsters monsters;
        public Waves waves;
        public Items ItemDatabase;
        public NPCs NPCsDatabase;

        private static Database instance;

        public Database()
        {
            MYSQL = new MySQLHandler();
            try
            {
                if (MYSQL.Connection.State != ConnectionState.Open)
                    MYSQL.Connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("MSG_FATAL", "Can't connect to database. " + ex.ToString());
                return;
            }

            Console.WriteLine("MSG_STARTUP", "Connected to MYSQL.");

            skills = new Skills(MYSQL);
            Console.WriteLine("MSG_LOAD", "Skill database loaded with [" + skills.SkillList.Count + "] skills");
            ItemDatabase = new Items(MYSQL);
            Console.WriteLine("MSG_LOAD", "Item database loaded with [" + ItemDatabase.ItemList.Count + "] items");
            monsters = new Monsters(MYSQL);
            Console.WriteLine("MSG_LOAD", "Monster database loaded with [" + monsters.MonsterListDB.Count + "] monsters");
            waves = new Waves(MYSQL, new Monsters(MYSQL));
            Console.WriteLine("MSG_LOAD", "Waves database loaded with [" + waves.WaveList.Count + "] waves");
            NPCsDatabase = new NPCs(MYSQL, ItemDatabase);
            Console.WriteLine("MSG_LOAD", "NPC database loaded with [" + NPCsDatabase.NPCsList.Count + "] NPCs!");
            Maps = new maps();
        }

        public static Database Instance
        {
            get
            {
                if (instance == null)
                    instance = new Database();
                return instance;
            }
        }
    }
}

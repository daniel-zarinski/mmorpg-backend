using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace LoginServer
{
    public class Skill
    {
        private int id;
        private int attack;
        private int level;
        private string name;
        private bool active;

        public Skill(int _id, int _attack, int _level, string _name)
        {
            id = _id;
            attack = _attack;
            level = _level;
            name = _name;
            active = false;
        }

        public Skill(Skill sk)
        {
            id = sk.Id;
            attack = sk.Attack;
            level = sk.Level;
            name = sk.Name;
            active = sk.Active;
        }
        public int Id
        {
            get
            {
                return id;
            }
        }
        
        public int Attack
        {
            get
            {
                return attack;
            }
        }

        public int Level
        {
            get { return level;  }
            set { level = value; }
        }
        public string Name
        {
            get { return name; }
        }

        public bool Active {
            get { return active; }
            set { active = value; }
        }
    }

    class Skills
    {
        private List<Skill> skillList;

        public Skills(MySQLHandler dataBase)
        {
            skillList = new List<Skill>();

            if (dataBase.Connection.State != ConnectionState.Open)
            {
                try
                {
                    dataBase.Connection.Open();
                }
                catch
                {
                    //
                }
            }

            MySqlCommand query = dataBase.Connection.CreateCommand();
            query.CommandText = "SELECT * FROM `skills`";

            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Skill skill = new Skill(
                            reader.GetInt32("id"),
                            reader.GetInt32("attack"),
                            reader.GetInt32("level"),
                            reader.GetString("name")
                            );
                        skillList.Add(skill);
                    }
                }
            }
            catch
            {
                //
            }
        }

        public Skill GetSkillByID(int id)
        {
            for (int i = 0; i < skillList.Count; i++)
            {
                if (skillList[i].Id == id)
                {
                    //Skill sk = new Skill(skillList[i]);
                    //return sk;
                    return skillList[i];
                }
            }
            return null;
        }

        public List<Skill> SkillList
        {
            get
            {
                return skillList;
            }
        }
    }
}

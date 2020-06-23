using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace LoginServer
{
    public class Wave
    {
        // monsters
        // which map
        // their coordinates
        public int[] MonsterIDs { get; set; }
        public int[] MonsterCount { get; set; }
        public int[] MonsterX { get; set; }
        public int[] MonsterY { get; set; }
        public int WaveID { get; set; }
        public List<Monster> MonstersInWave { get; set; }
        public int MapID { get; set; }


        public Wave(int waveid, int[] monsterids, int[] monstercount, int[] x, int[] y, List<Monster> moninwave, int mapid)
        {
            this.MonsterIDs = new int[monsterids.Length];
            this.MonsterIDs = monsterids;

            this.MonsterCount = new int[monstercount.Length];
            this.MonsterCount = monstercount;

            this.MonsterX = new int[x.Length];
            this.MonsterX = x;

            this.MonsterY = new int[y.Length];
            this.MonsterY = y;

            this.WaveID = waveid;
            this.MonstersInWave = moninwave;
            MapID = mapid;
        }

        public Wave()
        {
            this.WaveID = -1;
        }
    }

    public class Waves
    {
        private List<Wave> wavelist;

        public Waves(MySQLHandler dataBase, Monsters monstersdb)
        {
            wavelist = new List<Wave>();
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
            query.CommandText = "SELECT * FROM `wavelist`";

            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<Monster> mons = new List<Monster>();
                        string[] tmpmonsterids = reader.GetString("monsters").Split(',');
                        string[] tmpmonstercount = reader.GetString("monstercount").Split(',');
                        string[] tmpx = reader.GetString("x").Split(',');
                        string[] tmpy = reader.GetString("y").Split(',');

                        int[] monids = new int[tmpmonsterids.Length];
                        int[] moncount = new int[tmpmonsterids.Length];
                        int[] x = new int[tmpmonsterids.Length];
                        int[] y = new int[tmpmonsterids.Length];
                        for (int i=0; i < tmpmonsterids.Length; i ++)
                        {
                            monids[i] = int.Parse(tmpmonsterids[i]);
                            moncount[i] = int.Parse(tmpmonstercount[i]);
                            x[i] = int.Parse(tmpx[i]);
                            y[i] = int.Parse(tmpy[i]);
                            float tmpspacing = 0;
                            for (int j = 0; j < moncount[i]; j++)
                            {
                                Monster tmpmon = new Monster();
                                tmpspacing = (float) (j) / 10.0f;
                                tmpmon = monstersdb.GetMonsterByID(monids[i]);
                                tmpmon.position.XCoord = x[i] + tmpspacing;
                                tmpmon.position.YCoord = y[i];
                                mons.Add(tmpmon);
                            }
                        }
                        Wave wave = new Wave(
                            reader.GetInt32("id"),
                            monids,
                            moncount,
                            x,
                            y,
                            mons,
                            reader.GetInt32("mapid"));
                        /*for (int i = 0; i < mons.Count; i++)
                            MessageBox.Show(mons[i].Test.ToString());*/
                        wavelist.Add(wave);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        public Wave getWaveByID(int id)
        {
            for (int i = 0; i < wavelist.Count; i++)
            {
                if (wavelist[i].WaveID == id)
                    return wavelist[i];
            }

            return new Wave();
        }

        public List<Wave> WaveList
        {
            get { return wavelist; }
        }

    }
}

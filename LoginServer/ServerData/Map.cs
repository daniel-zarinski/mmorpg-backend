using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginServer
{
    public class Map
    {
        public List<Character> CharactersInMap { get; set; }
        public List<Monster> MonstersInMap { get; set; }
        public int WaveNumber { get; set; }
        public bool Active { get; set; } // WorldThread - means monsters have spawned
        public bool GameStarted { get; set; } // Character teleported to map, and now beginning to spawn monsters.
        public Map()
        {
            this.CharactersInMap = new List<Character>();
            this.MonstersInMap = new List<Monster>();
            this.WaveNumber = 0;
            this.Active = false;
            GameStarted = false;
        }
    }

    public class maps
    {
        private List<Map> maplist;

        public maps()
        {
            maplist = new List<Map>();
        }

        public List<Map> MapList
        {
            get { return maplist; }
        }

        public void CreateNewWave(int wavenumber, Waves w)
        {
            lock (maplist)
            {
                Wave wave = w.getWaveByID(wavenumber);
                for (int i = 0; i < wave.MonstersInWave.Count; i++)
                {
                    maplist[maplist.Count - 1].MonstersInMap.Add(wave.MonstersInWave[i]);
                }
            }
        }

        public void AddToMap(Character c)
        {
            lock (maplist)
            {
                maplist.Add(new Map());
                maplist[maplist.Count - 1].CharactersInMap.Add(c);
                c.MapID = maplist.Count - 1;
            }

        }

        public void AddToMap(Monster m, int mapid)
        {
            m.MapID = mapid;
            maplist[mapid].MonstersInMap.Add(m);
        }

        public void RemoveFromMap(Character c)
        {
            // idk yet
        }
    }
}

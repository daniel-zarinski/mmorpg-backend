using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer
{
    public class WorldHandler
    {
        
        public static int GetMonsterSpawnsByWave(int wave)
        {
            switch(wave)
            {
                case 1:
                    return 3;
                default:
                    return wave + 2;
            }
        }

        public static string SendMonsterPak(int wave)
        {
            //SendToAllConnected("SPAWNM;3;1;" + thisclient.WaveNumber);
            return "SPAWNM;" + GetMonsterSpawnsByWave(wave) + ";" + "1;" + wave + ";3" + ";0";
        }
    }
}

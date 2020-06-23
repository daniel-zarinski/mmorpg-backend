using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer
{
    public abstract class Creature
    {
        protected string name;
        protected ulong id;
        protected int level;
        protected uint str;
        protected uint dex;
        protected uint _int;
        protected uint luck;
        protected uint maxhp;
        protected uint maxmp;
        protected uint curhp;
        protected uint curmp;
        protected int mapid;
        protected float Xpos;
        protected float Ypos;
    }
}

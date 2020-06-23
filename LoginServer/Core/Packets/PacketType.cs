using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer
{
    public enum PacketType
    {
        LOGIN, LOGINERROR, RECEIVE_CHARACTERS, INVENTORY, PICKUPITEM, CHANGESLOT, PICKUPGOLD, GETWORLD, RMOVE, SHOOT, SKILLS, ACTIVESKILL, LEVELUPSKILL, ACTIVATESKILL, STATUP, CHAT, USERCONNECTED, SPAWNM, WAVESELECT
        , BUYITEM
    }
}

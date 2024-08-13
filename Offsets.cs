using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeTrainer
{
    internal class Offsets
    {
        public static string GameProcess = "ac_client.exe";
        public static string offsetLocalPlayer = GameProcess+"+18AC00";

        public static string EntityList = GameProcess+"+18AC04";
        public static string ViewMatrix = GameProcess+ "+17DFD0"; // 1:"+17E05C" 2:"+17DFD0" 3:"+17E060" 4:"+17DFF0"

        public static string m_XPos = "0x4";
        public static string m_YPos = "0x8";
        public static string m_ZPos = "0xC";

        public static string m_ViewAngleX = "0x34";
        public static string m_ViewAngleY = "0x38";
        public static string m_FOV = "0x334";
        public static string m_FOV_SCOPE = "0x338";

        public static string m_Health = "0xEC";
        public static string m_Armor = "0xF0";
        public static string m_Ammo = "0x140";
        public static string m_Grenades = "0x144";
        public static string m_KillsNumber = "0x1DC";
        public static string m_Name = "0x205";
        public static string m_Team = "0x30C";

    }
}

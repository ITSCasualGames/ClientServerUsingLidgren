using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerConsole
{
    public class PlayerData
    {
        public string header = string.Empty;
        public string imageName = string.Empty;
        public string playerID;
        public float X;
        public float Y;

        public PlayerData(string messageHeader, string ImgName, string id, float x, float y)
        {
            header = messageHeader;
            playerID = id;
            imageName = ImgName;
            X = x;
            Y = y;
        }

        public string PlayerMessage(string header)
        {
            return header + ":" + playerID + ":" + X.ToString() + ":" + Y.ToString();
        }
    }
}

using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using GameServerConsole;
using Microsoft.Xna.Framework.Graphics;

namespace LingrenGame
{
    

    public class GamePlayer
    {
        string playerID;
        Vector2 position = new Vector2();
        NetClient _client;
        PlayerData _playerDataPacket;
        bool joined;

        public string ImageName = string.Empty;

        public GamePlayer(NetClient client,string ImgName, string playerid, Vector2 StartPos)
        {
            // Created as a reult of a joined message
            position = StartPos;
            playerID = playerid;
            ImageName = ImgName;
            _playerDataPacket = new PlayerData("Joined", ImgName, playerid, StartPos.X, StartPos.Y
                );

        }

        public GamePlayer(NetClient client, Guid playerid, string ImgName, Vector2 StartPos)
        {

            position = StartPos;
            playerID = playerid.ToString();
            ImageName = ImgName;
            // consruct a join player packet and serialise it
            _playerDataPacket = new PlayerData("Join", ImageName, PlayerID, StartPos.X,StartPos.Y);
            string json = JsonConvert.SerializeObject(_playerDataPacket);
            // construct the outgoing message
            NetOutgoingMessage sendMsg = client.CreateMessage();
            sendMsg.Write(json);
            client.SendMessage(sendMsg, NetDeliveryMethod.ReliableOrdered);


        }
        public PlayerData PlayerDataPacket
        {
            get
            {
                return _playerDataPacket;
            }

            set
            {
                _playerDataPacket = value;
                position.X = value.X;
                position.Y = value.Y;
            }
        }

        public string PlayerID
        {
            get
            {
                return playerID;
            }

            set
            {
                playerID = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public bool Joined
        {
            get
            {
                return joined;
            }

            set
            {
                joined = value;
            }
        }

        public void ChangePosition(Vector2 newPosition)
        {
            position = newPosition;

        }
    }
}

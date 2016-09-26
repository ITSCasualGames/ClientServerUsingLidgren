using Lidgren.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServerConsole
{
    class Program
    {
        static NetPeerConfiguration config = new NetPeerConfiguration("myGame")
        {
            Port = 12345
        };
        static NetServer server;
        static List<PlayerData> Players = new List<PlayerData>();

        static void Main(string[] args)
        {
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.StatusChanged);

            server = new NetServer(config);
            config.AutoFlushSendQueue = true;
            server.Start();
            //server.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));
            //server.MessageReceivedEvent.WaitOne();
            //for the server
            for (;;)
            {
                // Stop the fan from going around needlessly
                server.MessageReceivedEvent.WaitOne();
                NetIncomingMessage msgIn;
                while ((msgIn = server.ReadMessage()) != null)
                {
                    //create message type handling with a switch
                    switch (msgIn.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            //This type handles all data that has been sent by you.
                            // broadcast message to all clients
                            var inMess = msgIn.ReadString();
                            process(inMess);
                            //NetOutgoingMessage reply = server.CreateMessage();
                            //reply.Write(inMess);
                            //foreach (NetConnection client in server.Connections)
                            //    server.SendMessage(reply, client, NetDeliveryMethod.ReliableOrdered);
                            break;
                        //All other types are for library related events (some examples)
                        case NetIncomingMessageType.DiscoveryRequest:
                            Console.WriteLine("Discovery Request from Client");
                            NetOutgoingMessage msg = server.CreateMessage();
                            //add a string as welcome text
                            msg.Write("Greetings from " + config.AppIdentifier + " server ");
                            //send a response
                            server.SendDiscoveryResponse(msg, msgIn.SenderEndPoint);
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            msgIn.SenderConnection.Approve();
                            break;

                        
                        case NetIncomingMessageType.StatusChanged:

                            switch ((NetConnectionStatus)msgIn.ReadByte())
                            {
                                case NetConnectionStatus.Connected:
                                    Console.WriteLine("{0} Connected", msgIn.SenderConnection);
                                    break;
                                case NetConnectionStatus.Disconnected:
                                    Console.WriteLine("{0} Disconnected", msgIn.SenderConnection);

                                    break;
                                case NetConnectionStatus.RespondedAwaitingApproval:
                                    msgIn.SenderConnection.Approve();
                                    break;
                            }
                            break;
                        default:
                            Console.WriteLine("unhandled message with type: "
                                + msgIn.MessageType);
                            break;
                    }
                    //Recycle the message to create less garbage
                    server.Recycle(msgIn);
                }
            }

        }

        private static void process(string inMess)
        {
            Console.WriteLine("Data " + inMess);
            PlayerData p = JsonConvert.DeserializeObject<PlayerData>(inMess);
            switch(p.header)
            {
                case "Join":
                    // Add the player to the server copy
                    Players.Add(p);
                    // send the message to all clients that players are joined
                    foreach (PlayerData player in Players)
                    {
                        PlayerData joined = new PlayerData("Joined",player.imageName, player.playerID, player.X, player.Y);
                        string json = JsonConvert.SerializeObject(joined);
                        //
                        NetOutgoingMessage JoinedMessage = server.CreateMessage();
                        JoinedMessage.Write(json);
                        server.SendToAll(JoinedMessage, NetDeliveryMethod.ReliableOrdered);
                    }

                    break;
                default:
                    break;
            }
        }

        private static void GotMessage(object state)
        {
            //use local message variable
            NetIncomingMessage msgIn;
            //create message type handling with a switch
            msgIn = server.ReadMessage();
                switch (msgIn.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        //This type handles all data that has been sent by you.
                        // broadcast message to all clients
                        var inMess = msgIn.ReadString();
                        NetOutgoingMessage reply = server.CreateMessage();
                        reply.Write(inMess);
                        foreach (NetConnection client in server.Connections)
                            server.SendMessage(reply, client, NetDeliveryMethod.ReliableOrdered);
                        break;
                    //All other types are for library related events (some examples)
                    case NetIncomingMessageType.DiscoveryRequest:
                        NetOutgoingMessage msg = server.CreateMessage();
                        //add a string as welcome text
                        msg.Write("Greetings from " + config.AppIdentifier + " server ");
                        //send a response
                        server.SendDiscoveryResponse(msg, msgIn.SenderEndPoint);
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        msgIn.SenderConnection.Approve();
                        break;

                }
                //Recycle the message to create less garbage
                server.Recycle(msgIn);
        }
    }
}

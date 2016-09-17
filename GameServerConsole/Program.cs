using Lidgren.Network;
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
            //LocalAddress = System.Net.IPAddress.Parse("127.0.0.1"),
            Port = 12345
            
        };
        static NetServer server;

        static void Main(string[] args)
        {
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);

            server = new NetServer(config);
            config.AutoFlushSendQueue = true;
            server.Start();
            //server.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));
            //server.MessageReceivedEvent.WaitOne();
            //for the server
            for (;;)
            {
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

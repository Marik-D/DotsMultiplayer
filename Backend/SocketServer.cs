using System;
using System.Net;
using System.Net.Sockets;
using DotsCore;
using NetCoreServer;

namespace Backend
{
    public class SocketServer : WsServer
    {
        public ServerState ServerState = new ServerState();
        
        public SocketServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new PlayerSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat WebSocket server caught an error with code {error}");
        }
    }
}
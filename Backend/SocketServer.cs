using System;
using System.Net;
using System.Net.Sockets;
using NetCoreServer;

namespace Backend
{
    public class SocketServer : WsServer
    {
        public SocketServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new SocketSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat WebSocket server caught an error with code {error}");
        }
    }
}
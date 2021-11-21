using System;
using System.Net.Sockets;
using System.Text;
using DotsCore;
using NetCoreServer;
using Newtonsoft.Json;

namespace Backend
{
    public class SocketSession : WsSession
    {
        private readonly SocketServer _server;
        public SocketSession(SocketServer server) : base(server)
        {
            _server = server;
        }

        public override void OnWsConnected(HttpRequest request)
        {
            Console.WriteLine($"Chat WebSocket session with Id {Id} connected!");

            // Send invite message
            var message = JsonConvert.SerializeObject(_server.GameState.BoardState);
            this.SendText(message);
        }

        public override void OnWsDisconnected()
        {
            Console.WriteLine($"Chat WebSocket session with Id {Id} disconnected!");
        }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            var move = JsonConvert.DeserializeObject<Move>(message);
            
            Console.WriteLine("Got move: " + move);
            
            _server.GameState.BoardState.Place(move.Row, move.Col);
            
            Console.WriteLine("Current move = " + _server.GameState.BoardState.CurrentMove);
            
            this.SendText(JsonConvert.SerializeObject(_server.GameState.BoardState));
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat WebSocket session caught an error with code {error}");
        }
    }
}
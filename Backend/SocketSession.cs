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
        private BoardState _gameState = new BoardState(15, 15);
        
        public SocketSession(WsServer server) : base(server) { }

        public override void OnWsConnected(HttpRequest request)
        {
            Console.WriteLine($"Chat WebSocket session with Id {Id} connected!");

            // Send invite message
            var message = JsonConvert.SerializeObject(_gameState);
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
            
            _gameState.PlaceByPlayer(new CellPos(move.Row, move.Col), move.Player);
            
            this.SendText(JsonConvert.SerializeObject(_gameState));
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat WebSocket session caught an error with code {error}");
        }
    }
}
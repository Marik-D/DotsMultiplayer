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
        private readonly JsonRpc _rpc;
        
        public SocketSession(SocketServer server) : base(server)
        {
            _server = server;
            
            this._rpc = new JsonRpc(msg =>
            {
                Console.WriteLine("Sent message: " + msg);
                this.SendText(msg);
            });
            this._rpc.Handle<Move, object>("MakeMove", move =>
            {
                _server.GameState.BoardState.Place(move.Row, move.Col);

                this._rpc.Call<BoardState, object>("UpdateBoardState", _server.GameState.BoardState);

                return null;
            });
        }

        public override void OnWsConnected(HttpRequest request)
        {
            this._rpc.Call<BoardState, object>("UpdateBoardState", _server.GameState.BoardState);
        }

        public override void OnWsDisconnected()
        {
            Console.WriteLine($"Chat WebSocket session with Id {Id} disconnected!");
        }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Got message: " + message);
            
            this._rpc.HandleMessageFromTransport(message);
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat WebSocket session caught an error with code {error}");
        }
    }
}
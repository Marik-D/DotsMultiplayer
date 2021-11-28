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

        private GameState _game; 
        
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
                _game.MakeMove(move);
                return null;
            });
        }

        public void AssignGame(GameState state)
        {
            this._game = state;
            this._rpc.Call<ClientState, object>("UpdateClientState", ClientState.Playing);
            this._rpc.Call<BoardState, object>("UpdateBoardState", _game.BoardState);
        }

        public override void OnWsConnected(HttpRequest request)
        {
            this._rpc.Call<ClientState, object>("UpdateClientState", ClientState.Matchmaking);
            _server.ServerState.OnPlayerConnected(this);
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

        public void UpdateBoardState(BoardState boardState)
        {
            this._rpc.Call<BoardState, object>("UpdateBoardState", boardState);
        }
    }
}
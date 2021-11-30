using System;
using System.Net.Sockets;
using System.Text;
using DotsCore;
using NetCoreServer;
using Newtonsoft.Json;

namespace Backend
{
    public class PlayerSession : WsSession
    {
        private readonly SocketServer _server;
        private readonly JsonRpc _rpc;

        private GameState _game;

        public string PlayerName;
        
        public PlayerSession(SocketServer server) : base(server)
        {
            _server = server;
            
            this._rpc = new JsonRpc(msg =>
            {
                this.SendText(msg);
            });
            this._rpc.Handle<Move, object>("MakeMove", move =>
            {
                Console.WriteLine($"PlayerSession[{Id}] Made move {move}");
                _game.MakeMove(move);
                return null;
            });
            this._rpc.Handle<string, object>("JoinMatchmaking", name =>
            {
                PlayerName = name;
                Console.WriteLine($"PlayerSession[{Id}] Joined matchmaking: name={name}");
                return null;
            });
        }

        public void AssignGame(GameState state)
        {
            Console.WriteLine($"PlayerSession[{Id}] Game stated: gameId={state.Id}");
            this._game = state;
            this._rpc.Call<ClientState, object>("UpdateClientState", ClientState.Playing);
            this._rpc.Call<BoardState, object>("UpdateBoardState", _game.BoardState);
        }

        public override void OnWsConnected(HttpRequest request)
        {
            Console.WriteLine($"PlayerSession[{Id}] Connected");
            this._rpc.Call<ClientState, object>("UpdateClientState", ClientState.Matchmaking);
            _server.ServerState.OnPlayerConnected(this);
        }

        public override void OnWsDisconnected()
        {
            Console.WriteLine($"PlayerSession[{Id}] Disconnected");
        }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            
            this._rpc.HandleMessageFromTransport(message);
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"PlayerSession[{Id}] Caught an error {error}");
        }

        public void UpdateBoardState(BoardState boardState)
        {
            this._rpc.Call<BoardState, object>("UpdateBoardState", boardState);
        }
    }
}
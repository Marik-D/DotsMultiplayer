using System;
using System.Threading.Tasks;
using DotsCore;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class ServerConnection
    {
        public Text gameStateLabel;
        public Image gameStateLabelContainer;
        
        private readonly WebSocket _socket;
        private readonly JsonRpc _rpc;

        public WebSocketState State => _socket.State;
        
        public event Action<BoardState> BoardStateUpdated;
        public event Action<ClientState> ClientStateUpdated;

        public ServerConnection(string endpoint)
        {
            this._socket = new WebSocket(endpoint);
            
            _socket.OnOpen += () =>
            {
                Debug.Log("Connection open!");

                _rpc.Call<string, object>("JoinMatchmaking", StateManager.PlayerName);
            };

            _socket.OnError += (e) =>
            {
                Debug.Log("Error! " + e);
            };

            _socket.OnClose += (e) =>
            {
                Debug.Log("Connection closed!");
            };

            _socket.OnMessage += (bytes) =>
            {
                // getting the message as a string
                var message = System.Text.Encoding.UTF8.GetString(bytes);
                Debug.Log("received message " + message);
                
                this._rpc.HandleMessageFromTransport(message);
            };
            
            this._rpc = new JsonRpc(msg => this._socket.SendText(msg));
            
            this._rpc.Handle<ClientState>("UpdateClientState", state =>
            {
                Debug.Log("Client state updated " + state);

                this.ClientStateUpdated?.Invoke(state);
            });

            this._rpc.Handle<BoardState>("UpdateBoardState", state =>
            {
                this.BoardStateUpdated?.Invoke(state);
            });
        }

        public void Connect()
        {
            // Returned task will block until connection closes.
            _socket.Connect();
        }
        
        public void Update()
        {
            #if !UNITY_WEBGL || UNITY_EDITOR
                _socket.DispatchMessageQueue();
            #endif
        }

        public async Task Close()
        {
            await _socket.Close();
        }

        public async Task MakeMove(Move move)
        {
            await this._rpc.Call<Move, object>("MakeMove", move);
        }
        
        public async Task FinishGame()
        {
            await this._rpc.Call<object, object>("FinishGame", null);
        }
    }
}
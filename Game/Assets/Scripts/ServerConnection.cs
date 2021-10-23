using System.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;

namespace DefaultNamespace
{
    public class ServerConnection
    {
        readonly WebSocket _socket;

        public ServerConnection(string endpoint)
        {
            this._socket = new WebSocket(endpoint);
            
            _socket.OnOpen += () =>
            {
                Debug.Log("Connection open!");
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
                Debug.Log("OnMessage! " + message);
            };
        }

        public async Task Connect()
        {
            await _socket.Connect();
        }
        
        public void Update()
        {
            #if !UNITY_WEBGL || UNITY_EDITOR
                _socket.DispatchMessageQueue();
            #endif
        }

        public async Task SendWebSocketMessage()
        {
            if (_socket.State == WebSocketState.Open)
            {
                await _socket.SendText("plain text message");
            }
        }

        public async Task Close()
        {
            await _socket.Close();
        }
    }
}
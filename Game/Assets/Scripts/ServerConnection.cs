using System.Threading.Tasks;
using DotsCore;
using NativeWebSocket;
using UnityEngine;

namespace DefaultNamespace
{
    public class ServerConnection
    {
        readonly WebSocket _socket;

        public WebSocketState State => _socket.State;

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
            var message = JsonUtility.ToJson(move);
            Debug.Log("json message: " + message);
            await _socket.SendText(message);
        }
    }
}
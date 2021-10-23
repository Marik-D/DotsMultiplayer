using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class SocketBehaviour : MonoBehaviour
{
    ServerConnection _connection = new ServerConnection("ws://localhost:8080");
    
    // Start is called before the first frame update
    async void Start()
    {
        _connection.Connect();
        
        Debug.Log("Connecting");
        
        InvokeRepeating(nameof(SendWebSocketMessage), 0.0f, 0.3f);
    }
    
    async void SendWebSocketMessage()
    {
        Debug.Log("Send message");
        await _connection.SendWebSocketMessage();
    }

    // Update is called once per frame
    void Update()
    {
        _connection.Update();
    }
    
    private void OnApplicationQuit()
    {
        _connection.Close();
    }
}

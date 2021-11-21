using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DotsCore;
using NativeWebSocket;
using UnityEngine;

public class SocketBehaviour : MonoBehaviour
{
    public ServerConnection Connection = new ServerConnection("ws://localhost:8080");
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting");
        Connection.Connect();
        
    }

    // Update is called once per frame
    void Update()
    {
        Connection.Update();
    }
    
    private void OnApplicationQuit()
    {
        Connection.Close();
    }
}

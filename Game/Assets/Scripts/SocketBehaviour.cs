using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DotsCore;
using NativeWebSocket;
using UnityEngine;
using UnityEngine.UI;

public class SocketBehaviour : MonoBehaviour
{
    public Text gameStateLabel;
    public Image gameStateLabelContainer;
    
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

        Connection.ClientStateUpdated += state =>
        {
            if (state == ClientState.Matchmaking)
            {
                gameStateLabel.text = "Matchmaking...";
            }
            else if (state == ClientState.Playing)
            {
                gameStateLabelContainer.gameObject.SetActive(false);
            }
        };
    }
    
    private void OnApplicationQuit()
    {
        Connection.Close();
    }
}

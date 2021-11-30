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
    public SpriteRenderer boardRenderer;
    public GameObject inGameUi;
    
    public ServerConnection Connection = new ServerConnection("ws://localhost:8080");
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting");
        Connection.Connect();

        boardRenderer.enabled = false;
        inGameUi.SetActive(false);
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
                boardRenderer.enabled = true;
                inGameUi.SetActive(true);
                gameStateLabelContainer.gameObject.SetActive(false);
            }
        };
    }
    
    private void OnApplicationQuit()
    {
        Connection.Close();
    }
}

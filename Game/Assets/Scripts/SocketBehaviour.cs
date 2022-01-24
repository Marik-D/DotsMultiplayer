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
    public GameInfoUi gameInfoUi;
    public GameObject inGameUiContainer;

    private const bool Remote = true;
    
    public ServerConnection Connection = new ServerConnection(Remote ? "ws://142.93.162.57:8080" : "ws://localhost:8080");
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting");
        Connection.Connect();

        boardRenderer.enabled = false;
        gameInfoUi.SetVisible(false);
    }

    // Update is called once per frame
    void Update()
    {
        Connection.Update();

        Connection.ClientStateUpdated += state =>
        {
            StateManager.ClientState = state;
            if (state.State == ClientState.StateEnum.Matchmaking)
            {
                gameStateLabel.text = "Matchmaking...";
            }
            else if (state.State == ClientState.StateEnum.Playing)
            {
                boardRenderer.enabled = true;
                inGameUiContainer.SetActive(true);
                gameInfoUi.SetVisible(true);
                gameInfoUi.SetNames(state);
                gameStateLabelContainer.gameObject.SetActive(false);
            }
            else if (state.State == ClientState.StateEnum.GameOver)
            {
                inGameUiContainer.SetActive(false);
                gameStateLabelContainer.gameObject.SetActive(true);
                gameStateLabel.text = $"Game over. Winner: {state.Winner}";
            }
        };
    }
    
    private void OnApplicationQuit()
    {
        Connection.Close();
    }
}

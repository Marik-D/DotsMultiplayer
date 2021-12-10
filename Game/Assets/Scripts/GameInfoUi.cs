using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DotsCore;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoUi : MonoBehaviour
{
    public Text player1Text;
    public Text player2Text;

    private ClientState _clientState = new ClientState();
    private BoardState _boardState;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);    
    }
    
    public void SetNames(ClientState state)
    {
        _clientState = state;
        UpdateText();
    }

    public void SetBoardState(BoardState state)
    {
        _boardState = state;
        player1Text.fontStyle = state.CurrentPlacer == Player.Red ? FontStyle.BoldAndItalic : FontStyle.Normal;
        player2Text.fontStyle = state.CurrentPlacer == Player.Blue ? FontStyle.BoldAndItalic : FontStyle.Normal;
        UpdateText();
    }

    private void UpdateText()
    {
        player1Text.text = $"{_clientState.Player1Name}{(StateManager.MyPlayer == Player.Red ? " (me)" : "")} - {_boardState?.RedScore ?? 0}";
        player2Text.text = $"{_clientState.Player2Name}{(StateManager.MyPlayer == Player.Blue ? " (me)" : "")} - {_boardState?.BlueScore ?? 0}";
    }

}

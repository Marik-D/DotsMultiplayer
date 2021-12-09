using System.Collections;
using System.Collections.Generic;
using DotsCore;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoUi : MonoBehaviour
{
    public Text player1Text;
    public Text player2Text;
    
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
        player1Text.text = state.Player1Name;
        player2Text.text = state.Player2Name;
    }

    public void SetTurn(Player currentTurn)
    {
        player1Text.fontStyle = currentTurn == Player.Red ? FontStyle.BoldAndItalic : FontStyle.Normal;
        player2Text.fontStyle = currentTurn == Player.Blue ? FontStyle.BoldAndItalic : FontStyle.Normal;
    }

}

using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayClick()
    {
        SceneManager.LoadScene("Scenes/GameScene", LoadSceneMode.Single);
    }

    public void OnExitClick()
    {
        Application.Quit();
    }

    public void OnNameChanged(string name)
    {
        Debug.Log($"Name changed: {name}");
        StateManager.PlayerName = name;
    }
}

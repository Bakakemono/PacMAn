using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SceneManagerNetwork : MonoBehaviour {

    public void LobbyStartGame()
    {
        NetworkManager.singleton.ServerChangeScene("GameSceneNetwork");
    }

    public void GameTrackerWin()
    {
        NetworkManager.singleton.ServerChangeScene("LooseSceneNetwork");
    }

    public void GameRunnerWin()
    {
        NetworkManager.singleton.ServerChangeScene("WinSceneNetwork");
        
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene("LobbyNetwork");
    }

    public void Exit()
    {
        Application.Quit();
    }
}

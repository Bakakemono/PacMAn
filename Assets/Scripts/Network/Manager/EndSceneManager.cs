using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class EndSceneManager : NetworkBehaviour {

    public void ExitGame()
    {
        Application.Quit();
    }
}

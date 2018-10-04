using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkManagerManager : NetworkManager {

    private void Update()
    {
        Scene CurrentScene = SceneManager.GetActiveScene();

        if(networkSceneName == "bla")
        {

        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        Debug.Log("spawn");

        //NetworkServer.AddPlayerForConnection(conn, playerPrefab, playerControllerId);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager {

    [SerializeField] private GameObject SlotSelection;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        //base.OnServerAddPlayer(conn, playerControllerId);

        if (SceneManager.GetActiveScene().name == "GameSceneNetwork")
        {
            GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
            Debug.Log("player");
        }

        else if (SceneManager.GetActiveScene().name == "LobbyNetwork")
        {
            GameObject LobbyIcon = (GameObject)Instantiate(SlotSelection, Vector3.zero, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, LobbyIcon, playerControllerId);

        }
        
    }
}

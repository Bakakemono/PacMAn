using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkBehaviour {


    public SyncListInt currentNodes = new SyncListInt();

    public SyncListInt targetNodes = new SyncListInt();

    public SyncListBool playerNmb;

    private int nmbPlayer = 5;

    //Start-------------Start
	void Awake () {
        for(int i = 0; i < nmbPlayer; i++)
        {
            currentNodes.Add(0);
            currentNodes.Add(0);

            targetNodes.Add(0);
            targetNodes.Add(0);

            playerNmb.Add(false);
        }
	}



    [Command]
    public void CmdNodePosition(Vector2 _currentNode, Vector2 _targetNode, int _playerNmb)
    {
        currentNodes[_playerNmb * 2]     = Mathf.RoundToInt(_currentNode.x);
        currentNodes[_playerNmb * 2 + 1] = Mathf.RoundToInt(_currentNode.y);

        targetNodes[_playerNmb * 2]     = Mathf.RoundToInt(_targetNode.x);
        targetNodes[_playerNmb * 2 + 1] = Mathf.RoundToInt(_targetNode.y);
    }
}

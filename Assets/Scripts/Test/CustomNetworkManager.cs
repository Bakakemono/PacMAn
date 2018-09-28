using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkBehaviour {


    [SyncVar]
    public float MusicTime = 0;

    //public SyncListInt currentNodes = new SyncListInt();
    public Vector2Int[] currentNodes;

    //public SyncListInt targetNodes = new SyncListInt();
    public Vector2Int[] targetNodes;

    //public bool[] playerNmb;
    [SyncVar] public int numberPlayer = 0;

    private int nmbPlayer = 5;

    //Awake-------------Awake
    void Awake()
    {
        //for (int i = 0; i < nmbPlayer; i++)
        //{
        //    playerNmb.Add(false);
        //}

        currentNodes = new Vector2Int[nmbPlayer];
        targetNodes = new Vector2Int[nmbPlayer];
        //playerNmb = new bool[nmbPlayer];
        

        //for (int i = 0; i < nmbPlayer; i++)
        //    playerNmb[i] = false;
        //MusicTime = FindObjectOfType<GameManager>().MusicSource.time;
    }


    

    //[Command]
    //public void CmdNodePosition(Vector2 _currentNode, Vector2 _targetNode, int _playerNmb)
    //{
    //    Debug.Log("recieve");
    //    int cX = Mathf.RoundToInt(_currentNode.x);
    //    int cY = Mathf.RoundToInt(_currentNode.y);
    //    int tX = Mathf.RoundToInt(_targetNode.x);
    //    int tY = Mathf.RoundToInt(_targetNode.y);

        
    //    RpcNodePosition(cX, cY, tX, tY, _playerNmb);
    //}

    [ClientRpc]
    public void RpcNodePosition(int cX, int cY, int tX, int tY, int _nmbPlayer)
    {
        currentNodes[_nmbPlayer] = new Vector2Int(cX, cY);

        targetNodes[_nmbPlayer] = new Vector2Int(tX, tY);
    }

    //[Command]
    //public void CmdPlayerNmbUpdate(int _nmbPlayer)
    //{
    //    playerNmb[_nmbPlayer] = true;
    //    RpcPlayerNmbUpdate(_nmbPlayer);
    //}

    [ClientRpc]
    public void RpcPlayerNmbUpdate(int _nmbPlayer)
    {
        //for(int i = 0; i <= _nmbPlayer; i++)
        //playerNmb[_nmbPlayer] = true;
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class SlotPlayer : NetworkBehaviour {

    private GameObject GoButtom;

    private NetworkContainer networkContainer;

    [SyncVar] private int playerSlot = -1;

    [SerializeField] private GameObject PacMan;
    [SerializeField] private GameObject Ghost;

    [SerializeField] private float interval = 3;
    private bool isPacman = false;
    private bool isInPlace = false;

    [SyncVar] Color color;

    PlayerNumber playerNumber;
    
	// Use this for initialization
	void Start () {
        if (isLocalPlayer)
            playerNumber = FindObjectOfType<PlayerNumber>();


        GoButtom = GameObject.Find("GoButton");
        if (isServer && isLocalPlayer)
        {
            GoButtom.transform.localPosition = new Vector3(0, 0, 0);
            GoButtom.SetActive(false);
        }

        networkContainer = FindObjectOfType<NetworkContainer>();

        if (isLocalPlayer && !isServer)
        {
            color = new Color(Random.value, Random.value, Random.value);
            CmdSyncColor(color);
            if (!networkContainer.playerTwo)
            {
                SetupPlayerSlot(1);
            }
            else if (!networkContainer.playerThree)
            {
                SetupPlayerSlot(2);
            }
            else if (!networkContainer.playerFour)
            {
                SetupPlayerSlot(3);
            }
            else if (!networkContainer.playerFive)
            {
                SetupPlayerSlot(4);
            }
            else
            {
                Debug.LogError("Too much player");
                Destroy(gameObject);
            }
        }

        if(isLocalPlayer && isServer)
        {
            Debug.Log("PlayerOne");
            SetupPlayerSlot(0);
        }
    }
    
    // Update is called once per frame
    void Update () {
        if (!isInPlace)
        {
            if (playerSlot == 0)
            {
                isInPlace = true;
                isPacman = true;
                transform.position = Vector2.zero;
                ChoosePlayerType(isPacman);
                SavePlayerSlot(0);
            }
            else if (playerSlot == 1)
            {
                isInPlace = true;
                ChoosePlayerType(isPacman);
                transform.position = new Vector2(interval, 0);
                SavePlayerSlot(1);

            }
            else if (playerSlot == 2)
            {
                isInPlace = true;
                ChoosePlayerType(isPacman);
                transform.position = new Vector2(0, interval);
                SavePlayerSlot(2);
            }
            else if (playerSlot == 3)
            {
                isInPlace = true;
                ChoosePlayerType(isPacman);
                transform.position = new Vector2(-interval, 0);
                SavePlayerSlot(3);
            }
            else if (playerSlot == 4)
            {
                isInPlace = true;
                ChoosePlayerType(isPacman);
                transform.position = new Vector2(0, -interval);
                SavePlayerSlot(4);
            }
        }
    }

    private void FixedUpdate()
    {
        if(isLocalPlayer && isServer)
            if (CheckAllPlayerReady())
                GoButtom.SetActive(true);
            else
                GoButtom.SetActive(false);
    }

    [Command]
    private void CmdSyncPlayerSelected(int _playerSelect)
    {
        playerSlot = _playerSelect;
            if (_playerSelect == 0)
            {
                networkContainer.playerOne = true;
            }
            else if (_playerSelect == 1)
            {
                networkContainer.playerTwo = true;
            }
            else if (_playerSelect == 2)
            {
                networkContainer.playerThree = true;
            }
            else if (_playerSelect == 3)
            {
                networkContainer.playerFour = true;
            }
            else if (_playerSelect == 4)
            {
                networkContainer.playerFive = true;
            }
    }

    [Command]
    private void CmdSyncColor(Color _color)
    {
        color = _color;
    }


    private void SetupPlayerSlot(int _playerSlot)
    {
        playerSlot = _playerSlot;
        CmdSyncPlayerSelected(_playerSlot);
    }
    
    private void ChoosePlayerType(bool _isPacman)
    {
        PacMan.SetActive(_isPacman);
        Ghost.SetActive(!_isPacman);
    }

    private bool CheckAllPlayerReady()
    {
        return (networkContainer.playerOne && networkContainer.playerTwo && networkContainer.playerThree && networkContainer.playerFour && networkContainer.playerFive);
    }


    [Command]
    private void CmdOnPlayerDisconnected(int _playerSlot)
    {
        if (_playerSlot == 0)
        {
            networkContainer.playerOne = false;
        }
        else if (_playerSlot == 1)
        {
            networkContainer.playerTwo = false;
        }
        else if (_playerSlot == 2)
        {
            networkContainer.playerThree = false;
        }
        else if (_playerSlot == 3)
        {
            networkContainer.playerFour = false;
        }
        else if (_playerSlot == 4)
        {
            networkContainer.playerFive = false;
        }
    }

    private void SavePlayerSlot(int _playerSlot)
    {
        if (isLocalPlayer)
            playerNumber.playerSlotNumber = _playerSlot;
    }

    public override void OnNetworkDestroy()
    {
        CmdOnPlayerDisconnected(playerSlot);
        base.OnNetworkDestroy();
    }
}

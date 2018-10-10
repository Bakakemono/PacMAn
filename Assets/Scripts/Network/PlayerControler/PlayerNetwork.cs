using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetwork : NetworkBehaviour {

    //Le Rigidbody du player
    private Rigidbody2D rigid;

    //Entree du player pour savoir dans quelle direction il souhaite aller
    private Vector2 input;

    //La dernière direction du player enregistre en attente d'utilisation
    private Vector2Int newDirection = new Vector2Int(0, 0);
    //L'ancienne direction du player valable tant que "newDirection" ne la remplace pas
    private Vector2Int direction = new Vector2Int(0, 0);

    //vitesse generale du player
    [SerializeField] private float speed = 1;

    //Script de creation du player
    private BoardCreator board;

    //Tableau contenant toute la zone de jeux avec les nodes accessible ou non par le player
    private Node[,] Grid;
    //Le node duquel est parti le player
    private Node currentNode;
    //Le node vers lequel le player se dirige
    private Node targetNode;
    //La taille du player
    private float radius = 0.5f;
    //Tableau contenant tout les attanquant que la coureuse doit eviter
    private GameObject[] Trackers;
    //Booleen servant a savoir si le player est ou non la coureuse
    private bool PlayerGirl = false;

    //Varaible syncronisee servant à connaitre sont numero de player et donc sont spawn
    [SyncVar] private int playerSlot = 0;

    private SpriteRenderer Sprite;

    private NetworkContainer networkContainer;

    private SceneManagerNetwork gameManagerNetwork;

    //Variable syncronisee pour partager le currentNode
    [SyncVar] private int CurX;
    [SyncVar] private int CurY;

    //Variable syncronisee pour partager le targetNode
    [SyncVar] private int TarX;
    [SyncVar] private int TarY;


    [SerializeField] private bool isDirectSpawn = false;

    // Use this for initialization
    void Start()
    {
        networkContainer = FindObjectOfType<NetworkContainer>();

        //Recherche si un slot est libre dans la partie
        if (isDirectSpawn)
        {
            if (isLocalPlayer && !isServer)
            {
                if (!networkContainer.playerTwo)
                {
                    SetupPlayer(1);
                }
                else if (!networkContainer.playerThree)
                {
                    SetupPlayer(2);
                }
                else if (!networkContainer.playerFour)
                {
                    SetupPlayer(3);
                }
                else if (!networkContainer.playerFive)
                {
                    SetupPlayer(4);
                }
                else
                {
                    Debug.LogError("Too much player");
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            if (isLocalPlayer)
            {
                playerSlot = FindObjectOfType<PlayerNumber>().playerSlotNumber;
                Debug.Log("playerSlot : " + playerSlot);
                SetupPlayer(playerSlot);
            }
        }

        if (playerSlot == 0 && isLocalPlayer && isServer && isDirectSpawn)
        {
            PlayerGirl = true;
            SetupPlayer(0);
        }

        rigid = GetComponent<Rigidbody2D>();
        board = FindObjectOfType<BoardCreator>();

        Grid = board.Grid;

        Sprite = GetComponent<SpriteRenderer>();

        AssignBegingingNode();

        if (isLocalPlayer && isServer)
            StartCoroutine(ShearchTracker());
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            //if (/*/*gameManager.Appeard*/)
            //{
            InputSelection();
            ConvertInput();
            GoToTargetClient();
        }
        if (isLocalPlayer && isServer)
        {

        }
        if (playerSlot == 0)
        {
            Sprite.color = Color.yellow;
            speed = 7;
        }
        else
        {
            Sprite.color = new Color(1, 0, 0, 1);
            if (gameObject.tag != "Tracker")
                gameObject.tag = "Tracker";
        }

        if (isLocalPlayer && !isServer)
        {
            Sprite.color = new Color(1, 0.4f, 0, 1);
        }

    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            SetupNodeNotLocalPlayer();
            GoToTargetNotLocalPlayer();
        }
    }

    private void InputSelection()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }

    private void ConvertInput()
    {
        if (input.x > 0.1f)
        {
            newDirection = new Vector2Int(1, 0);
        }

        else if (input.y > 0.1f)
        {
            newDirection = new Vector2Int(0, 1);
        }

        else if (input.x < -0.1f)
        {
            newDirection = new Vector2Int(-1, 0);
        }

        else if (input.y < -0.1f)
        {
            newDirection = new Vector2Int(0, -1);
        }
        

    }

    private void AssignBegingingNode()
    {
        if (isLocalPlayer)
        {
            if (PlayerGirl || playerSlot == 0)
            {
                currentNode = Grid[(Random.value > 0.5 ? 1 : 30), (Random.value > 0.5 ? 1 : 30)];
            }

            else
            {
                if (playerSlot == 1)
                    currentNode = Grid[14, 17];
                else if (playerSlot == 2)
                    currentNode = Grid[17, 17];
                else if (playerSlot == 3)
                    currentNode = Grid[14, 14];
                else if (playerSlot == 4)
                    currentNode = Grid[17, 14];
            }

            targetNode = currentNode;
            CmdSyncNode(currentNode.GridPosition, targetNode.GridPosition);

            transform.position = currentNode.Position;
        }
    }

    private void FindTargetClient()
    {
        Vector2Int target = currentNode.GridPosition + newDirection;
        Vector2Int oldTarget = currentNode.GridPosition + direction;

        if (target.x >= 0 && target.x < board.GridSize.x && target.y >= 0 && target.y < board.GridSize.y && !Grid[target.x, target.y].isWall)
        {
            direction = newDirection;
            targetNode = Grid[target.x, target.y];

            //Envoie que le joueur change de cap
            UpdateNode();
        }

        else if (oldTarget.x >= 0 && oldTarget.x < board.GridSize.x && oldTarget.y >= 0 && target.y < board.GridSize.y && !Grid[oldTarget.x, oldTarget.y].isWall)
        {
            targetNode = Grid[oldTarget.x, oldTarget.y];

            //Envoie que le joueur garde sont cap
            UpdateNode();
        }
    }

    private void GoToTargetClient()
    {

        rigid.velocity = (targetNode.Position - new Vector2(transform.position.x, transform.position.y)).normalized * speed;
        //if (gameManager.finish)
        //    rigid.velocity = new Vector2(0, 0);

        //Permet au joueur de changer de direction sur la même ligne
        if ((direction + newDirection) == Vector2Int.zero)
        {
            rigid.velocity = new Vector2(0, 0);

            Vector2Int tmpdir = direction;
            direction = newDirection;
            newDirection = tmpdir;

            Node tmpNode = currentNode;
            currentNode = targetNode;
            targetNode = tmpNode;

            //Envoie du changement de direction
            UpdateNode();
        }

        //Check si le player est arrive a destination ou si il se trouve a son point de depart
        if (Vector2.Distance(targetNode.Position, transform.position) < 0.1f || currentNode == targetNode)
        {
            transform.position = targetNode.Position;
            rigid.velocity = Vector2.zero;
            currentNode = targetNode;

            FindTargetClient();
        }
    }

    private void SetupNodeNotLocalPlayer()
    {
        currentNode = Grid[CurX, CurY];

        targetNode = Grid[TarX, TarY];
    }

    private void GoToTargetNotLocalPlayer()
    {
        rigid.velocity = (targetNode.Position - new Vector2(transform.position.x, transform.position.y)).normalized * speed;

        if (Vector2.Distance(targetNode.Position, transform.position) < 0.1f || targetNode.GridPosition == currentNode.GridPosition)
        {
            transform.position = targetNode.Position;
            rigid.velocity = Vector2.zero;
        }
    }

    private void UpdateNode()
    {
        CmdSyncNode(currentNode.GridPosition, targetNode.GridPosition);
    }

    [Command]
    private void CmdSyncPlayerNumber(int _playerNmb)
    {
        playerSlot = _playerNmb;
    }

    [Command]
    private void CmdSyncNode(Vector2 _currentNode, Vector2 _targetNode)
    {
        CurX = Mathf.RoundToInt(_currentNode.x);
        CurY = Mathf.RoundToInt(_currentNode.y);

        TarX = Mathf.RoundToInt(_targetNode.x);
        TarY = Mathf.RoundToInt(_targetNode.y);

    }

    [Command]
    private void CmdSyncPlayerSelected(int _playerSelect)
    {
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

    private void SetupPlayer(int _playerSlot)
    {
        playerSlot = _playerSlot;
        CmdSendTag();
        CmdSyncPlayerNumber(_playerSlot);
        CmdSyncPlayerSelected(_playerSlot);
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

    private void DisconnetPlayer()
    {
        if (isClient)
            CmdOnPlayerDisconnected(playerSlot);
    }

    [Command]
    private void CmdSendTag()
    {
        RpcSendTag();
    }

    [ClientRpc]
    private void RpcSendTag()
    {
        gameObject.tag = "Tracker";
    }

    private IEnumerator ShearchTracker()
    {
        yield return new WaitForSeconds(1);

        Trackers = GameObject.FindGameObjectsWithTag("Tracker");
        int playerNumber = 5;
        if(Trackers.Length == playerNumber)
        {
            StartCoroutine(CheckDistTracker());
        }
        else
        {
            StartCoroutine(ShearchTracker());
        }
    }

    private IEnumerator CheckDistTracker()
    {
        while(true)
        {
            foreach(GameObject T in Trackers)
            {
                if(T != gameObject && Vector3.Distance(T.transform.position, transform.position) < radius * 2)
                {
                    StartLooseTransition();
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private void StartLooseTransition()
    {
        FindObjectOfType<SceneManagerNetwork>().GameTrackerWin();
    }

    public override void OnNetworkDestroy()
    {
        DisconnetPlayer();
        Debug.Log("logout player : " + playerSlot);
        base.OnNetworkDestroy();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestPlayerNet : NetworkBehaviour {

    private Rigidbody2D rigid;
    private Vector2 input;
    private Vector2Int newDirection = new Vector2Int(0, 0);
    private Vector2Int direction = new Vector2Int(0, 0);

    [SerializeField] private float speed = 1;

    private BoardCreator board;
    private GameManager gameManager;

    private Node[,] Grid;
    private Node currentNode;
    private Node targetNode;
    private float radius = 0.5f;
    private List<Transform> Trackers;
    private bool PlayerGirl = false;


    [SyncVar] private int playerNmb = -1;
    private int nmbPlayer = 5;

    private SpriteRenderer Sprite;
    private CustomNetworkManager customNetworkManager;

    // Use this for initialization
    void Start()
    {
        customNetworkManager = FindObjectOfType<CustomNetworkManager>();

        if (isLocalPlayer && !isServer)
        {
            CmdPlayerNmbUpdate(0);

            for (int i = 1; i < nmbPlayer; i++)
            {
                
                if (customNetworkManager.numberPlayer == i)
                {
                    playerNmb = i;
                    //gameObject.tag = "tracker";
                    CmdUpdatPlayerNmb(i);
                    customNetworkManager.numberPlayer = i + 1;
                    Debug.Log("playerNmb Supposed : " + i);
                    break;
                }
            }
            if (playerNmb == -1)
                Destroy(gameObject);
        }
        Debug.Log("playerNMB    : " + playerNmb);

        if (playerNmb == -1)
        {
            CmdPlayerNmbUpdate(0);

            CmdUpdatPlayerNmb(0);
            //customNetworkManager.playerNmb[0] = true;

            playerNmb = 0;
            PlayerGirl = true;
        }

        rigid = GetComponent<Rigidbody2D>();
        board = FindObjectOfType<BoardCreator>();
        gameManager = FindObjectOfType<GameManager>();

        Grid = board.Grid;

        AssignBegingingNode();
        //if (PlayerGirl)
        //{
        //    Trackers = new List<Transform>();

        //    Trackers.Add(GameObject.FindWithTag("Tracker").transform);

        //}
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

            if (PlayerGirl)
            {
                //CheckLoose();
            }
        }
        if (playerNmb == 0)
            GetComponent<SpriteRenderer>().color = Color.blue;
        else
            GetComponent<SpriteRenderer>().color = Color.red;
        //}
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            SetupNodeNetwork();
            GoToTargetNotCLient();
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
        if (PlayerGirl || playerNmb == 0)
        {
            currentNode = Grid[(Random.value > 0.5 ? 1 : 30), (Random.value > 0.5 ? 1 : 30)];
        }

        else
        {
            if (playerNmb == 1)
                currentNode = Grid[14, 17];
            else if (playerNmb == 2)
                currentNode = Grid[17, 17];
            else if (playerNmb == 3)
                currentNode = Grid[14, 14];
            else if (playerNmb == 4)
                currentNode = Grid[14, 14];
        }

        targetNode = currentNode;
        transform.position = currentNode.Position;

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

        if (Vector2.Distance(targetNode.Position, transform.position) < 0.1f || currentNode == targetNode)
        {
            transform.position = targetNode.Position;
            rigid.velocity = Vector2.zero;
            currentNode = targetNode;

            FindTargetClient();
        }
    }

    private void SetupNodeNetwork()
    {
        Vector2Int _currentNodePos = new Vector2Int(customNetworkManager.currentNodes[playerNmb].x, customNetworkManager.currentNodes[playerNmb].y);
        currentNode = Grid[_currentNodePos.x, _currentNodePos.y];

        Vector2Int _targetNodePos = new Vector2Int(customNetworkManager.targetNodes[playerNmb].x, customNetworkManager.targetNodes[playerNmb].y);
        targetNode = Grid[_targetNodePos.x, _targetNodePos.y];
    }

    private void GoToTargetNotCLient()
    {
        rigid.velocity = (targetNode.Position - new Vector2(transform.position.x, transform.position.y)).normalized * speed;

        if (Vector2.Distance(targetNode.Position, transform.position) < 0.1f || currentNode == targetNode)
        {
            transform.position = targetNode.Position;
            rigid.velocity = Vector2.zero;
        }
    }

    private void UpdateNode()
    {
        CmdNodePosition(currentNode.GridPosition, targetNode.GridPosition, playerNmb);
        Debug.Log("send");
    }

    [Command]
    private void CmdUpdatPlayerNmb(int _playerNmb)
    {
        playerNmb = _playerNmb;
    }



    [Command]
    private void CmdNodePosition(Vector2 _currentNode, Vector2 _targetNode, int _playerNmb)
    {
        Debug.Log("recieve");
        int cX = Mathf.RoundToInt(_currentNode.x);
        int cY = Mathf.RoundToInt(_currentNode.y);
        int tX = Mathf.RoundToInt(_targetNode.x);
        int tY = Mathf.RoundToInt(_targetNode.y);


        customNetworkManager.RpcNodePosition(cX, cY, tX, tY, _playerNmb);
    }

    [Command]
    public void CmdPlayerNmbUpdate(int _nmbPlayer)
    {
        //customNetworkManager.RpcPlayerNmbUpdate(_nmbPlayer);
        customNetworkManager.numberPlayer = customNetworkManager.numberPlayer + 1;
    }
}

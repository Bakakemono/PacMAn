using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class TestPlayer : NetworkBehaviour {

    //   public int speed = 10;
    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {
    //       if (!isLocalPlayer)
    //           return;
    //       transform.position =    transform.position +
    //                               new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized * Time.deltaTime * speed;
    //}

    //   public override void OnStartLocalPlayer()
    //   {
    //       GetComponent<SpriteRenderer>().color = Color.blue;
    //   }



    public enum PlayerSelection
    {
        PLAYER_GIRL,
        PLAYER_TRACKER
    }

    [SerializeField]
    public
       PlayerSelection playerSelection = PlayerSelection.PLAYER_GIRL;

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

    private int playerNmb = 0;


    private CustomNetworkManager customNetworkManager;

    // Use this for initialization
    void Start()
    {
        customNetworkManager = GetComponent<CustomNetworkManager>();

        if (playerSelection == PlayerSelection.PLAYER_GIRL)
            PlayerGirl = true;

        rigid = GetComponent<Rigidbody2D>();
        board = FindObjectOfType<BoardCreator>();
        gameManager = FindObjectOfType<GameManager>();
        Grid = board.Grid;
        if (PlayerGirl)
        {
            currentNode = Grid[gameManager.spawnNodeA.x, gameManager.spawnNodeA.y];
        }

        else
        {
            currentNode = Grid[gameManager.spawnNodeB.x, gameManager.spawnNodeB.y];
        }

        targetNode = currentNode;
        transform.position = currentNode.Position;
        if (PlayerGirl)
        {
            Trackers = new List<Transform>();

            Trackers.Add(GameObject.FindWithTag("Tracker").transform);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.Appeard && isClient)
        {
            InputSelection();
            ConvertInput();
            GoToTargetClient();

            if (PlayerGirl)
            {
                CheckLoose();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isClient)
            return;
        SetupNodeNetwork();
        GoToTargetNotCLient();
    }

    private void InputSelection()
    {
        switch (playerSelection)
        {
            case PlayerSelection.PLAYER_GIRL:
                input.x = Input.GetAxisRaw("Horizontal");
                input.y = Input.GetAxisRaw("Vertical");
                break;

            case PlayerSelection.PLAYER_TRACKER:
                input.x = Input.GetAxisRaw("HorizontalTwo");
                input.y = Input.GetAxisRaw("VerticalTwo");
                break;
        }
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

    private void FindTarget()
    {
        Vector2Int target = currentNode.GridPosition + newDirection;
        Vector2Int oldTarget = currentNode.GridPosition + direction;

        if (target.x >= 0 && target.x < board.GridSize.x && target.y >= 0 && target.y < board.GridSize.y && !Grid[target.x, target.y].isWall)
        {
            direction = newDirection;
            targetNode = Grid[target.x, target.y];
        }

        else if (oldTarget.x >= 0 && oldTarget.x < board.GridSize.x && oldTarget.y >= 0 && target.y < board.GridSize.y && !Grid[oldTarget.x, oldTarget.y].isWall)
        {
            targetNode = Grid[oldTarget.x, oldTarget.y];

        }
    }

    private void GoToTargetClient()
    {

        rigid.velocity = (targetNode.Position - new Vector2(transform.position.x, transform.position.y)).normalized * speed;
        if (gameManager.finish)
            rigid.velocity = new Vector2(0, 0);

        if ((direction + newDirection) == Vector2Int.zero)
        {
            rigid.velocity = new Vector2(0, 0);

            Vector2Int tmpdir = direction;
            direction = newDirection;
            newDirection = tmpdir;

            Node tmpNode = currentNode;
            currentNode = targetNode;
            targetNode = tmpNode;

            customNetworkManager.CmdNodePosition(currentNode.GridPosition, targetNode.GridPosition, playerNmb);
        }

        if (Vector2.Distance(targetNode.Position, transform.position) < 0.1f || currentNode == targetNode)
        {
            transform.position = targetNode.Position;
            rigid.velocity = Vector2.zero;
            currentNode = targetNode;

            FindTarget();
        }
    }

    private void SetupNodeNetwork()
    {
        Vector2Int _currentNodePos = new Vector2Int(customNetworkManager.currentNodes[playerNmb * 2], customNetworkManager.currentNodes[playerNmb * 2 + 1]);
        currentNode = Grid[_currentNodePos.x, _currentNodePos.y];

        Vector2Int _targetNodePos = new Vector2Int(customNetworkManager.targetNodes[playerNmb * 2], customNetworkManager.targetNodes[playerNmb * 2 + 1]);
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
        customNetworkManager.CmdNodePosition(currentNode.GridPosition, targetNode.GridPosition, playerNmb);
    }

    private void CheckLoose()
    {
        if (!gameManager.loose)
            foreach (Transform t in Trackers)
            {
                if (Vector2.Distance(transform.position, t.position) < radius * 2)
                {
                    gameManager.loose = true;
                    break;
                }
            }
    }

}

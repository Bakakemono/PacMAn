using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    private Rigidbody2D rigid;
    private Vector2 input;
    private Vector2Int newDirection;
    private Vector2Int direction = new Vector2Int(0 ,0);

    [SerializeField] private float speed = 1;

    private BoardCreator board;
    private GameManager gameManager;

    private Node[,] Grid;
    private Node currentNode;
    private Node targetNode;

	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody2D>();
        board = FindObjectOfType<BoardCreator>();
        gameManager = FindObjectOfType<GameManager>();
        Grid = board.Grid;

        currentNode = Grid[gameManager.spawnLocation.x, gameManager.spawnLocation.y];
        targetNode = currentNode;
        transform.position = currentNode.Position;
	}
	
	// Update is called once per frame
	void Update () {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        FindInput();

        GoToTarget();
    }

    private void FindInput()
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



        if(target.x >= 0 && target.x < board.GridSize.x && target.y >= 0 && target.y < board.GridSize.y && !Grid[target.x, target.y].isWall)
        {
            direction = newDirection;
            targetNode = Grid[target.x, target.y];
        }
        else if(oldTarget.x >= 0 && oldTarget.x < board.GridSize.x && oldTarget.y >= 0 && target.y < board.GridSize.y && !Grid[oldTarget.x, oldTarget.y].isWall)
        {
            targetNode = Grid[oldTarget.x, oldTarget.y];
        }
    }

    private void GoToTarget()
    {
        rigid.velocity = (targetNode.Position - new Vector2(transform.position.x, transform.position.y)).normalized * speed;

        if (Vector2.Distance(targetNode.Position, transform.position) < 0.1f || currentNode == targetNode)
        {
            transform.position = targetNode.Position;
            rigid.velocity = new Vector2(0, 0);
            currentNode = targetNode;
            targetNode = currentNode;
            FindTarget();
        }

    }
}

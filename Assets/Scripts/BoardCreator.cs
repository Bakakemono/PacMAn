using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardCreator : MonoBehaviour {

    public Node[,] Grid;

    [SerializeField] public Vector2Int GridSize;
    [SerializeField] private Tilemap WallTileMap;

    private bool isRunning = false;

    private void Awake()
    {
        Grid = new Node[GridSize.x, GridSize.y];

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                Grid[x, y] = new Node(new Vector2Int(x, y), new Vector2(x + 0.5f, y + 0.5f), WallTileMap.HasTile(new Vector3Int(x, y, 0)));
            }   
        }
        isRunning = true;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (isRunning)
        {
            Gizmos.color = Color.red;

            foreach (Node n in Grid)
            {
                if(n.isWall)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;


                Gizmos.DrawSphere(new Vector3(n.Position.x, n.Position.y, 0), 0.1f);
            }
        }
    }

}

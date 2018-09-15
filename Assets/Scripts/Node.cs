using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public Vector2Int GridPosition;
    public Vector2 Position;
    public bool isWall;

    public Node(Vector2Int _GridPosition, Vector2 _Position, bool _isWall)
    {
        GridPosition = _GridPosition;
        Position = _Position;
        isWall = _isWall;
    }
}

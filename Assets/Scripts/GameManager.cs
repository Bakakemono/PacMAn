using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] GameObject PlayerGirl;
    private BoardCreator boardCreator;
    [SerializeField] public Vector2Int spawnLocation = new Vector2Int(16, 16);
    

	// Use this for initialization
	void Start () {
        boardCreator = FindObjectOfType<BoardCreator>();
        GameObject GirlPlayer = Instantiate(PlayerGirl, new Vector3(spawnLocation.x, spawnLocation.y, 0), transform.rotation);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

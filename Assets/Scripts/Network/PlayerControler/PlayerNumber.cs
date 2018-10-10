using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNumber : MonoBehaviour {

    public int playerSlotNumber;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashPoliceManager : MonoBehaviour {

    private Rigidbody2D rigid;

    private float speedRotation = 500;
    
	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        rigid.angularVelocity = speedRotation;
	}
}

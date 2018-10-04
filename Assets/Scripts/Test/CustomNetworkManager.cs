﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkBehaviour {


    [SyncVar]
    public float MusicTime = 0;


    //Ces booleens sont necessaire afin de differentier les players
    [SyncVar] public bool playerOne = false;
    [SyncVar] public bool playerTwo = false;
    [SyncVar] public bool playerThree = false;
    [SyncVar] public bool playerFour = false;
    [SyncVar] public bool playerFive = false;

    //Awake-------------Awake
    void Awake()
    {
        //MusicTime = FindObjectOfType<GameManager>().MusicSource.time;
    }
}

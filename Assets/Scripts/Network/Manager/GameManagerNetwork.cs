using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManagerNetwork : NetworkBehaviour {

    private float totalTime = 200;

    [SyncVar] private float time;
    [SerializeField] private Text TimerText;

    [SerializeField] private AudioSource MusicSource;
    [SerializeField] private AudioClip music;

    // Use this for initialization
    void Start () {
        if (isClient)
        {
            StartCoroutine(TrySyncMusic());
        }
        else if (isServer)
            CalutlateTimeLeft();

	}
	
	// Update is called once per frame
	void Update () {
        int timeLeft = Mathf.RoundToInt(music.length - MusicSource.time);
        TimerText.text = timeLeft.ToString();

        if(timeLeft <= 0)
        {
            if(isServer)
            {
                FindObjectOfType<SceneManagerNetwork>().GameRunnerWin();
            }
        }
    }

    [Server]
    private void CalutlateTimeLeft()
    {
        time = MusicSource.time;
    }

    [Client]
    private void SyncMusic()
    {
        MusicSource.time = time + NetworkManager.singleton.client.GetRTT() / 2000;
    }

    private IEnumerator TrySyncMusic()
    {
        yield return new WaitForSeconds(1);
        SyncMusic();
    }
}

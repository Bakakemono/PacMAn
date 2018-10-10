using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [SerializeField] private GameObject girlPlayer;
    [SerializeField] private GameObject trackerPlayer;

    private CameraManager gameCamera;
    [SerializeField] public Vector2Int spawnNodeA = new Vector2Int(1, 1);
    [SerializeField] public Vector2Int spawnNodeB = new Vector2Int(31, 31);
    public bool Appeard = false;

    Image TransitionPanel;
    Image PausePanel;
    private bool isPause = false;
    public AudioSource MusicSource;
    public AudioClip music;

    private bool Timefinish = false;
    private float timer = 0;
    [SerializeField] private Text timerText;

    public List<GameObject> Opponents;

    public bool loose = false;
    public bool finish = false;

    // Use this for initialization
    void Start() {
        TransitionPanel = GameObject.FindGameObjectWithTag("TransitionPanel").GetComponent<Image>();
        PausePanel = GameObject.FindGameObjectWithTag("PausePanel").GetComponent<Image>();
        PausePanel.gameObject.SetActive(false);

        gameCamera = FindObjectOfType<CameraManager>();

        GameObject TrackerPlayer = Instantiate(trackerPlayer, new Vector3(spawnNodeB.x, spawnNodeB.y, 0), transform.rotation);
        TrackerPlayer.GetComponent<SpriteRenderer>().color = Color.red;
        Opponents.Add(trackerPlayer);
        GameObject GirlPlayer = Instantiate(girlPlayer, new Vector3(spawnNodeA.x, spawnNodeA.y, 0), transform.rotation);
        GirlPlayer.GetComponent<SpriteRenderer>().color = Color.yellow;

        MusicSource = GameObject.Find("MusicSource").GetComponent<AudioSource>();
        music = MusicSource.clip;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPause)
        {
            PausePanel.gameObject.SetActive(true);
            MusicSource.Pause();
            isPause = true;
            Time.timeScale = 0;
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && isPause)
        {
            PausePanel.gameObject.SetActive(false);
            MusicSource.Play();
            isPause = false;
            Time.timeScale = 1;
        }
        if (!Appeard)
        {
            Color color = TransitionPanel.color;
            color.a = Mathf.Lerp(color.a, 0, 0.05f);
            TransitionPanel.color = color;
            if (TransitionPanel.color.a < 0.1f)
            {
                TransitionPanel.color = new Color(0, 0, 0, 0);
                Appeard = true;
            }
        }
        if (!MusicSource.isPlaying)
        {
            Timefinish = true;
        }
        SetTime();

        if (loose || Timefinish)
        {
            gameCamera.shakeDuration = 100;
            finish = true;
            Color color = TransitionPanel.color;
            color.a = Mathf.Lerp(color.a, 1, 0.05f);
            TransitionPanel.color = color;
            if (TransitionPanel.color.a > 0.95f)
                if (loose)
                    SceneManager.LoadScene("DefeatScene");
                else if (Timefinish)
                    SceneManager.LoadScene("WinScene");
        }


    }

    private void SetTime()
    {
        if (!Timefinish)
        {
            timer = music.length - MusicSource.time;
            timerText.text = Mathf.Round(timer).ToString();
            if (timer <= 0)
            {
                timer = 000;
            }
        }
    }
}
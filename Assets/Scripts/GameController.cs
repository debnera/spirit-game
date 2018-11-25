using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;

public class GameController : MonoBehaviour
{
    public float timeLimitSeconds = 60;
    public RotateAround rotatingCameraScript;
    public FreeLookCam freeLookCam;
    public Transform mainCameraTransform;
    public GameObject currentPlayer;
    public GameObject playerPrefab;
    public GameObject statuePrefab;
    public GameObject spawnPoint;

    public GameObject startUI;
    public GameObject gameUI;
    public GameObject endUI;

    public GameObject startScreenCameraFocusPoint;
    public Vector3 startScreenCameraOffset;

    public Text timerText;

    public float respawnDelay = 1;

    public float maxStatueScale = 3;
    public float statueScalingRate = 1;

    private float statueScale = 1;
    private bool scalingStatue = false;

    private bool respawning = false;
    private string previousStatueFilename;

    public int maxNumberOfLoadedStatues = 50;
    
    private float timer;
    private bool playing;

    public enum GameState { StartScreen, Playing, EndScreen };

    public GameState currentState = GameState.StartScreen;

    // Use this for initialization
    void Awake()
    {
        LoadAllStatues();
    }

	void Start ()
	{
	    SwitchUI(currentState);
	    if (currentState == GameState.Playing)
	    {
            StartGame();
	    }
	}

    void SwitchUI(GameState state)
    {
        startUI.SetActive(false);
        gameUI.SetActive(false);
        endUI.SetActive(false);
        switch (state)
        {
            case GameState.StartScreen:
                if (rotatingCameraScript)
                {
                    rotatingCameraScript.enabled = true;
                    rotatingCameraScript.target = startScreenCameraFocusPoint;
                    rotatingCameraScript.SetOffset(startScreenCameraOffset);
                }

                if (freeLookCam)
                {
                    freeLookCam.enabled = false;
                }
                startUI.SetActive(true);
                break;
            case GameState.Playing:
                if (rotatingCameraScript)
                {
                    rotatingCameraScript.enabled = false;
                }
                if (freeLookCam)
                {
                    freeLookCam.enabled = true;
                }
                gameUI.SetActive(true);
                break;
            case GameState.EndScreen:
                if (rotatingCameraScript)
                {
                    rotatingCameraScript.enabled = false;
                    rotatingCameraScript.target = currentPlayer;
                }

                if (freeLookCam)
                {
                    freeLookCam.enabled = true;
                }
                endUI.SetActive(true);
                break;
        }
    }

    void StartGame()
    {
        timer = timeLimitSeconds;
        playing = true;
        if (!currentPlayer)
        {
            currentPlayer = SpawnPlayer();
        }
        SetCameraTarget(currentPlayer);
        currentPlayer.GetComponent<PlayerController>().SetCamera(mainCameraTransform);
    }

	
	// Update is called once per frame
	void Update () {
	    switch (currentState)
	    {
	        case GameState.StartScreen:
	            StartScreenUpdate();
                break;
	        case GameState.Playing:
	            PlayingUpdate();
	            break;
	        case GameState.EndScreen:
	            EndScreenUpdate();
	            break;
	    }
    }

    void StartScreenUpdate()
    {
        if (Input.anyKeyDown)
        {
            currentState = GameState.Playing;
            StartGame();
            SwitchUI(currentState);
        }
    }

    void EndScreenUpdate()
    {
        if (Input.anyKeyDown)
        {
            currentState = GameState.StartScreen;
            SwitchUI(currentState);
        }
    }

    void PlayingUpdate()
    {
        if (scalingStatue)
        {
            IncreaseScale();
        }
        if (Input.GetKeyDown(KeyCode.R) && !respawning)
        {
            DisablePlayerControls();
            statueScale = 1;
            scalingStatue = true;
        }
        if (Input.GetKeyUp(KeyCode.R) && !respawning)
        {
            scalingStatue = false;
            respawning = true;
            MakeStatue();
            SaveStatue();
            Invoke("Respawn", respawnDelay);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            // Debug save
            SaveStatue();

        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            // Debug load
            LoadStatue(GlobalSettings.GetStatueSavePath() + previousStatueFilename);
        }

        timer -= Time.deltaTime;
        if (timerText && playing)
        {
            
            int intTime = (int) Math.Ceiling(timer);
            int minutes = intTime / 60;
            int seconds = intTime - (60*minutes);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (timer < 0)
        {
            MakeStatue();
            DisablePlayerControls();
            SaveStatue();
            currentState = GameState.EndScreen;
            SwitchUI(currentState);
            currentPlayer = null;
        }
    }

    void IncreaseScale()
    {
        if (statueScale > maxStatueScale) return;
        statueScale += Time.deltaTime * statueScalingRate;
        PlayerController controller = currentPlayer.GetComponent<PlayerController>();
        if (controller)
        {
            controller.SetScale(statueScale);
        }

    }

    public void SaveStatue()
    {
        if (currentPlayer)
        {
            var body = currentPlayer.GetComponentInChildren<Body>();
            if (body)
            {
                previousStatueFilename = GlobalSettings.GenerateStatueFilename();
                body.Save(GlobalSettings.GetStatueSavePath(), previousStatueFilename);
            }
            else
            {
                Debug.LogError("Cannot find a body to save!");
            }
        }
    }

    void LoadAllStatues()
    {
        int numLoaded = 0;
        foreach (var name in GlobalSettings.GetAllStatueFilenames())
        {
            Debug.Log("Loading " + name);
            LoadStatue(name);
            numLoaded++;
            if (numLoaded > maxNumberOfLoadedStatues)
                break;
        }
        Debug.Log("Loaded " + numLoaded + " statues!");
            
    }

    public void LoadStatue(String path)
    {
        //var pos = transform.position;
        //if (CurrentPlayer)
        //    pos = CurrentPlayer.transform.position;
        var newStatue = Instantiate(statuePrefab);
        //newStatue.transform.position = pos + new Vector3(0, 3, 0);
        var body = newStatue.GetComponentInChildren<Body>();
        body.Load(path);
        body.FreezeToStatue();
    }

    public void MakeStatue()
    {
        // Freeze current player
        if (currentPlayer)
        {
            Body body = currentPlayer.GetComponentInChildren<Body>();
            if (body)
            {
                body.FreezeToStatue();
            }
        }
    }

    public void Respawn()
    {
        respawning = false;
        currentPlayer = SpawnPlayer();
        SetCameraTarget(currentPlayer);
    }

    public void DisablePlayerControls()
    {
        if (currentPlayer)
        {
            PlayerController controller = currentPlayer.GetComponent<PlayerController>();
            if (controller)
            {
                controller.EnableMovementAnimation(false);
                controller.enabled = false;
            }
        }
    }

    public GameObject SpawnPlayer()
    {
        GameObject newPlayer = Instantiate(playerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        newPlayer.GetComponent<PlayerController>().SetCamera(mainCameraTransform);
        return newPlayer;
    }

    public void SetCameraTarget(GameObject target)
    {
        if (!freeLookCam)
        {
            Debug.LogError("GameController is missing reference to Player Camera!");
        }
        freeLookCam.SetTarget(target.transform);
    }
}

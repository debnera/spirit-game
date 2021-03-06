﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;

[RequireComponent(typeof(AudioSource))]
public class GameController : MonoBehaviour
{
    public AudioClip scaleStatueClip;
    public AudioClip makeStatueClip;

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

    public String collectMoreBodyPartsInfo = "Collect at least {0:0} more body parts!";
    public String becomeStatueInfo = "Hold E to become a statue!";

    public Text playerGuideText;
    public Text timerText;
    public Text highScoreText;
    public Text playerHighScoreText;
    public Text currentHeightText;
    public Text highScoreGongratulationsText;
    public float gongratulationsDisplayDuration = 3f;

    public float respawnDelay = 1;

    public int minBodyPartsToStatue;
    public float maxStatueScale = 3;
    public float statueScalingRate = 1;

    public float statueScale = 1;
    public bool scalingStatue = false;

    private bool respawning = false;
    private string previousStatueFilename;

    public int maxNumberOfLoadedStatues = 50;
    
    private float timer;
    private bool playing;

    public enum GameState { StartScreen, Playing, EndScreen };

    public GameState currentState = GameState.StartScreen;

    private AudioSource audioSource;
    private float highScore = 0;
    private float playerHighScore = 0;
    private int minStateSwitchDelay = 2;
    private bool canSwitchState;
    private int prevRequiredLimbs = -1;

    // Use this for initialization
    void Awake()
    {
        LoadAllStatues();
        audioSource = GetComponent<AudioSource>();
        highScoreGongratulationsText.enabled = false;
    }

	void Start ()
	{
	    SwitchUI(currentState);
	    if (currentState == GameState.Playing)
	    {
            StartGame();
	    }
	}

    void EnableStateSwitching()
    {
        
        canSwitchState = true;
    }

    void SwitchUI(GameState state)
    {
        audioSource.Stop();
        canSwitchState = false;  // Used to prevent the player accidentally starting the game / respawning at the end
        Invoke("EnableStateSwitching", minStateSwitchDelay);
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
        if (Input.anyKeyDown && canSwitchState)
        {
            currentState = GameState.Playing;
            StartGame();
            SwitchUI(currentState);
        }
    }

    void EndScreenUpdate()
    {
        if (Input.anyKeyDown && canSwitchState)
        {
            currentState = GameState.StartScreen;
            SwitchUI(currentState);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void PlayingUpdate()
    {
        if (scalingStatue && Input.GetKey(KeyCode.E))
        {
            IncreaseScale();
        }
        else
        {
            scalingStatue = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetKeyDown(KeyCode.E) && !respawning && CanBecomeStatue())
        {
            DisablePlayerControls();
            statueScale = 1;
            scalingStatue = true;
            audioSource.clip = scaleStatueClip;
            audioSource.Play();
        }
        if (Input.GetKeyUp(KeyCode.E) && !respawning && CanBecomeStatue())
        {
            scalingStatue = false;
            respawning = true;
            audioSource.clip = makeStatueClip;
            audioSource.Play();
            MakeStatue();
            SaveStatue();
            CheckHighScore();
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

        if (highScoreText)
        {
            highScoreText.text = string.Format("{0:0.0} m", highScore);
        }

        if (playerHighScoreText)
        {
            playerHighScoreText.text = string.Format("{0:0.0} m", playerHighScore);
        }

        if (currentHeightText && currentPlayer)
        {
            currentHeightText.text = string.Format("{0:0.0} m", currentPlayer.transform.position.y);
        }

        if (playerGuideText)
        {
            int minFontSize = playerGuideText.fontSize;
            int maxFontSize = Mathf.RoundToInt(minFontSize * 1.4f);
            if (CanBecomeStatue())
            {
                
                playerGuideText.text = becomeStatueInfo;
            }
            else
            {
                playerGuideText.text = string.Format(collectMoreBodyPartsInfo, GetLimbsRemainingToBecomeStatue());
            }

            if (prevRequiredLimbs != GetLimbsRemainingToBecomeStatue())
            {
                //StartCoroutine(HighlightText(playerGuideText, 2, minFontSize, maxFontSize));
            }
            prevRequiredLimbs = GetLimbsRemainingToBecomeStatue();
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

    void CheckHighScore()
    {
        if (!currentPlayer) return;
        float height = currentPlayer.transform.position.y;
        if (height > highScore)
        {
            highScore = height;
            StartCoroutine(FadeFloatingText(highScoreGongratulationsText, gongratulationsDisplayDuration));
        }

        if (height > playerHighScore)
        {
            playerHighScore = height;
        }
    }

    void IncreaseScale()
    {
        if (statueScale > maxStatueScale)
        {
            audioSource.Stop();
            return;
        }
        statueScale += Time.deltaTime * statueScalingRate;
        PlayerController controller = currentPlayer.GetComponent<PlayerController>();
        if (controller)
        {
            controller.SetScale(statueScale);
        }

    }

    bool CanBecomeStatue()
    {
        PlayerController controller = currentPlayer.GetComponent<PlayerController>();
        if (!controller) return false;
        return GetLimbsRemainingToBecomeStatue() <= 0;
    }

    int GetLimbsRemainingToBecomeStatue()
    {
        // Returns the amount of Body Parts the player must still collect before becoming a statue
        PlayerController controller = currentPlayer.GetComponent<PlayerController>();
        if (!controller) return -1;
        return minBodyPartsToStatue - controller.GetNumberOfConnectedLimbs();
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
        highScore = Mathf.Max(highScore, body.transform.position.y);
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

            Rigidbody rb = currentPlayer.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.constraints = RigidbodyConstraints.None;
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

    public IEnumerator FadeFloatingText(Text text, float duration)
    {
        text.enabled = true;
        var color = text.color;
        color.a = 1;
        while (color.a > 0.0f)
        {
            color.a -= (Time.deltaTime / duration);
            text.color = color;
            yield return null;
        }
    }

    public IEnumerator HighlightText(Text text, float duration, int minFontSize, int maxFontSize)
    {
        text.enabled = true;
        text.fontSize = maxFontSize;
        float size = text.fontSize;
        float diff = maxFontSize - minFontSize;
        while (size > minFontSize)
        {
            size -= (Time.deltaTime / duration) * diff;
            text.fontSize = Mathf.FloorToInt(size);
            yield return null;
        }
        
    }
}

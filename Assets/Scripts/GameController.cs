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
    public FreeLookCam PlayerCamera;
    public Transform PlayerCameraTransform;
    public GameObject CurrentPlayer;
    public GameObject PlayerPrefab;
    public GameObject StatuePrefab;
    public GameObject SpawnPoint;

    public GameObject StartUI;
    public GameObject GameUI;
    public GameObject EndUI;

    public Text TimerText;

    public float RespawnDelay = 1;

    private bool Respawning = false;
    private string PreviousStatueFilename;

    
    private float timer;
    private bool Playing;

    public enum GameState { StartScreen, Playing, EndScreen };

    public GameState CurrentState = GameState.StartScreen;

    // Use this for initialization
    void Awake()
    {
        LoadAllStatues();
    }

	void Start ()
	{
	    SwitchUI(CurrentState);
	    if (CurrentState == GameState.Playing)
	    {
            StartGame();
	    }
	}

    void SwitchUI(GameState state)
    {
        StartUI.SetActive(false);
        GameUI.SetActive(false);
        EndUI.SetActive(false);
        switch (state)
        {
            case GameState.StartScreen:
                StartUI.SetActive(true);
                break;
            case GameState.Playing:
                GameUI.SetActive(true);
                break;
            case GameState.EndScreen:
                GameUI.SetActive(true);
                break;
        }
    }

    void StartGame()
    {
        timer = 60;
        Playing = true;
        if (!CurrentPlayer)
        {
            CurrentPlayer = SpawnPlayer();
        }
        SetCameraTarget(CurrentPlayer);
        CurrentPlayer.GetComponent<PlayerController>().SetCamera(PlayerCameraTransform);
    }

	
	// Update is called once per frame
	void Update () {
	    switch (CurrentState)
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
            CurrentState = GameState.Playing;
            StartGame();
            SwitchUI(CurrentState);
        }
    }

    void EndScreenUpdate()
    {
        if (Input.anyKeyDown)
        {
            CurrentState = GameState.StartScreen;
            SwitchUI(CurrentState);
        }
    }

    void PlayingUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R) && !Respawning)
        {
            DisablePlayerControls();
        }
        if (Input.GetKeyUp(KeyCode.R) && !Respawning)
        {
            Respawning = true;
            MakeStatue();
            SaveStatue();
            Invoke("Respawn", RespawnDelay);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            // Debug save
            SaveStatue();

        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            // Debug load
            LoadStatue(GlobalSettings.GetStatueSavePath() + PreviousStatueFilename);
        }

        if (TimerText && Playing)
        {
            timer -= Time.deltaTime;
            int intTime = (int) Math.Ceiling(timer);
            int minutes = intTime / 60;
            int seconds = intTime - (60*minutes);
            TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void SaveStatue()
    {
        if (CurrentPlayer)
        {
            var body = CurrentPlayer.GetComponentInChildren<Body>();
            if (body)
            {
                PreviousStatueFilename = GlobalSettings.GenerateStatueFilename();
                body.Save(GlobalSettings.GetStatueSavePath(), PreviousStatueFilename);
            }
            else
            {
                Debug.LogError("Cannot find a body to save!");
            }
        }
    }

    void LoadAllStatues()
    {
        foreach (var name in GlobalSettings.GetAllStatueFilenames())
        {
            Debug.Log("Loading " + name);
            LoadStatue(name);
        }
            
    }

    public void LoadStatue(String path)
    {
        //var pos = transform.position;
        //if (CurrentPlayer)
        //    pos = CurrentPlayer.transform.position;
        var newStatue = Instantiate(StatuePrefab);
        //newStatue.transform.position = pos + new Vector3(0, 3, 0);
        var body = newStatue.GetComponentInChildren<Body>();
        body.Load(path);
        body.FreezeToStatue();
    }

    public void MakeStatue()
    {
        // Freeze current player
        if (CurrentPlayer)
        {
            Body body = CurrentPlayer.GetComponentInChildren<Body>();
            if (body)
            {
                body.FreezeToStatue();
            }
        }
    }

    public void Respawn()
    {
        Respawning = false;
        CurrentPlayer = SpawnPlayer();
        SetCameraTarget(CurrentPlayer);
    }

    public void DisablePlayerControls()
    {
        if (CurrentPlayer)
        {
            PlayerController controller = CurrentPlayer.GetComponent<PlayerController>();
            if (controller)
            {
                controller.enabled = false;
            }
        }
    }

    public GameObject SpawnPlayer()
    {
        GameObject newPlayer = Instantiate(PlayerPrefab, SpawnPoint.transform.position, SpawnPoint.transform.rotation);
        newPlayer.GetComponent<PlayerController>().SetCamera(PlayerCameraTransform);
        return newPlayer;
    }

    public void SetCameraTarget(GameObject target)
    {
        if (!PlayerCamera)
        {
            Debug.LogError("GameController is missing reference to Player Camera!");
        }
        PlayerCamera.SetTarget(target.transform);
    }
}

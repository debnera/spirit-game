using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class GameController : MonoBehaviour
{
    public FreeLookCam PlayerCamera;
    public Transform PlayerCameraTransform;
    public GameObject CurrentPlayer;
    public GameObject PlayerPrefab;
    public GameObject StatuePrefab;
    public GameObject SpawnPoint;

    public float RespawnDelay = 1;

    private bool Respawning = false;
    private string PreviousStatueFilename;

	// Use this for initialization
    void Awake()
    {
        LoadAllStatues();
    }

	void Start () {
	    if (!CurrentPlayer)
	    {
	        CurrentPlayer = SpawnPlayer();
	    }
        SetCameraTarget(CurrentPlayer);
	    CurrentPlayer.GetComponent<PlayerController>().SetCamera(PlayerCameraTransform);
    }
	
	// Update is called once per frame
	void Update () {

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
                body.enabled = false;
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

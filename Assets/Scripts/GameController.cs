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
    public GameObject SpawnPoint;

    public float RespawnDelay = 1;

    private bool RespawnPressed = false;

	// Use this for initialization
	void Start () {
	    if (!CurrentPlayer)
	    {
	        CurrentPlayer = SpawnPlayer();
	    }
        SetCameraTarget(CurrentPlayer);
	}
	
	// Update is called once per frame
	void Update () {

	    if (Input.GetKeyDown(KeyCode.R) && !RespawnPressed)
	    {
	        RespawnPressed = true;
	        DisablePlayerControls();
	    }
	    if (Input.GetKeyUp(KeyCode.R) && RespawnPressed)
	    {
	        RespawnPressed = false;
	        MakeStatue();
            Invoke("Respawn", RespawnDelay);
	    }

        if (Input.GetKey(KeyCode.K))
	    {
	        // Debug save
	        var body = GetComponentInChildren<Body>();
	        if (body)
	        {
	            body.Save(GlobalSettings.GetStatueSavePath(), "test.statue");
	        }
	    }

	    if (Input.GetKey(KeyCode.L))
	    {
	        // Debug load
	        var newStatue = Instantiate(PlayerPrefab);
	        newStatue.transform.position = transform.position + new Vector3(0, 3, 0);
	        var body = newStatue.GetComponent<Body>();
	        body.Load(GlobalSettings.GetStatueSavePath() + Path.DirectorySeparatorChar + "test.statue");
	        body.FreezeToStatue();
	    }
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class FootstepGenerator : MonoBehaviour
{
	
	public GameObject footprint;
	public GameObject spawningPosition;
	public float footprintDistance;

	private Transform footprintParent;
	private Vector3 lastPrintPosition;
    private List<GameObject> footprints;

    private String filename = "foot.steps";

	// Use this for initialization
	void Start ()
	{
		lastPrintPosition = transform.position;
	    footprintParent = new GameObject().transform;
	    footprints = new List<GameObject>(1000);
        LoadFootstepsFromDirectory(GlobalSettings.GetFootstepSavePath());
    }
	
	// Update is called once per frame
	void Update ()
	{
		if (Vector3.Distance(transform.position, lastPrintPosition) > footprintDistance)
		{
			lastPrintPosition = transform.position;
			var obj = Instantiate(footprint, spawningPosition.transform.position, Quaternion.Euler(90,0,0), footprintParent);
            footprints.Add(obj);
        }
	}

    void OnApplicationQuit()
    {
        String name = GenerateFootstepFilename();
        SaveFootsteps(GlobalSettings.GetFootstepSavePath(), name);
    }

    void SaveFootsteps(String path, String filename)
    {
        String fullpath = path + Path.DirectorySeparatorChar + filename;
        Directory.CreateDirectory(path);
        FileStream fs = new FileStream(fullpath, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(fs, GetPositions());
            Debug.Log("Footsteps saved to " + path);
        }
        catch (SerializationException e)
        {
            Debug.Log("Saving failed:" + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
    }

    void LoadFootstepsFromDirectory(String path)
    {
        foreach (var name in GetAllFootstepFilenames(path))
            LoadFootstepsFromFile(name);
    }

    String GenerateFootstepFilename()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int unix_timestamp = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        return unix_timestamp + ".steps";
    }

    List<GameObject> LoadFootstepsFromFile(String path)
    {
        if (!File.Exists(path))
        {
            Debug.Log("Path does not exist: " + path);
            return new List<GameObject>();
        }
            
        List<SerializableVector3> positions = new List<SerializableVector3>();
        FileStream fs = new FileStream(path, FileMode.Open);
        try
        {
            Debug.Log("Loading " + path);
            BinaryFormatter formatter = new BinaryFormatter();
            positions = (List<SerializableVector3>)formatter.Deserialize(fs);
        }
        catch (Exception e)
        {
            Debug.Log("Loading failed: " + e.Message);
        }
        finally
        {
            fs.Close();
        }
        return PositionsToFootprints(positions);
    }

    List<SerializableVector3> GetPositions()
    {
        List<SerializableVector3> positions = new List<SerializableVector3>(footprints.Count);
        foreach (var obj in footprints)
        {
            positions.Add(obj.transform.position);
        }
        return positions;
    }

    List<GameObject> PositionsToFootprints(List<SerializableVector3> positions)
    {
        List<GameObject> objs = new List<GameObject>();
        Transform parent = new GameObject().transform;
        foreach (var pos in positions)
        {
            var obj = Instantiate(footprint, pos, Quaternion.Euler(90, 0, 0), parent);
            objs.Add(obj);
        }
        return objs;
    }

    List<String> GetAllFootstepFilenames(String path)
    {
        List<String> files = new List<string>();
        if (!Directory.Exists(path))
        {
            Debug.Log("Save folder not found - is this your first game?");
            return files;
        }
            
        foreach (string name in Directory.GetFiles(path))
        {
            if (name.EndsWith(".steps"))
                files.Add(name);
        }
        files.Sort();
        return files;
    }
}

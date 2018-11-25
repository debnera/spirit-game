using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GlobalSettings : MonoBehaviour {

    public static String GetSavefilePath()
    {
        return "SavedFiles";
    }

    public static String GetFootstepSavePath()
    {
        return GetSavefilePath() + Path.DirectorySeparatorChar + "footsteps" + Path.DirectorySeparatorChar;
    }

    public static String GetStatueSavePath()
    {
        return GetSavefilePath() + Path.DirectorySeparatorChar + "statues" + Path.DirectorySeparatorChar;
    }

    public static String GenerateStatueFilename()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int unix_timestamp = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
        String filename = unix_timestamp + ".statue";
        return filename;
    }

    public static List<String> GetAllStatueFilenames()
    {
        String path = GetStatueSavePath();
        List<String> files = new List<string>();
        if (!Directory.Exists(path))
        {
            Debug.Log("Save folder not found - is this your first game?");
            return files;
        }

        foreach (string name in Directory.GetFiles(path))
        {
            if (name.EndsWith(".statue"))
                files.Add(name);
        }
        files.Sort();
        files.Reverse();
        return files;
    }
}

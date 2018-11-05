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
}

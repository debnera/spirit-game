using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{

    public Text text;
    private DateTime endTime;

	// Use this for initialization
	void Start ()
	{
	    endTime = DateTime.Now.AddMinutes(1);
	}
	
	// Update is called once per frame
	void Update () {
	    if (text)
	    {
	        var time = endTime - DateTime.Now;
	        text.text = string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
	    }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepGenerator : MonoBehaviour
{
	
	public GameObject footprint;
	public GameObject spawningPosition;
	public float footprintDistance;

	private Transform footprintParent;
	private Vector3 lastPrintPosition;

	// Use this for initialization
	void Start ()
	{
		lastPrintPosition = transform.position;
		footprintParent = new GameObject().transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Vector3.Distance(transform.position, lastPrintPosition) > footprintDistance)
		{
			lastPrintPosition = transform.position;
			Instantiate(footprint, spawningPosition.transform.position, Quaternion.Euler(90,0,0), footprintParent);
		}
	}
}

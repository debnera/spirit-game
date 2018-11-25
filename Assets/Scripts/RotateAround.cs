using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    public float speed = 1f;
    public GameObject target;
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	    if (!target)
	        return;
	    transform.LookAt(target.transform);
	    transform.Translate(Vector3.right * Time.deltaTime * speed);
    }

    public void SetOffset(Vector3 offset)
    {
        if (!target)
            return;
        transform.position = target.transform.position + offset;
    }
}

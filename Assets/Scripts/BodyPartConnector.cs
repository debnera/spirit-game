using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartConnector : MonoBehaviour
{
    public BodyPart attachedPart;

    private FixedJoint joint;

	// Use this for initialization
	void Start ()
	{
	    joint = GetComponent<FixedJoint>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool IsEmpty()
    {
        return attachedPart != null;
    }

    public void AttachTo(BodyPart bodyPart)
    {
        attachedPart = bodyPart;
        bodyPart.SetAttached(true);
        //bodyPart.transform.parent = transform;
        joint.connectedBody = bodyPart.gameObject.GetComponent<Rigidbody>();
    }

    public void Detach()
    {
        if (attachedPart)
            attachedPart.SetAttached(false);
        attachedPart = null;
        joint.connectedBody = null;
    }
}

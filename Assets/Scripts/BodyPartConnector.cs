using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartConnector : MonoBehaviour
{
    public BodyPart attachedPart;

    private CharacterJoint joint;

	// Use this for initialization
	void Start ()
	{
	    joint = GetComponent<CharacterJoint>();
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
        IgnoreAllCollisions(bodyPart.gameObject);
        AdjustPosition(bodyPart);
        attachedPart = bodyPart;
        bodyPart.SetAttached(true);

        var j = bodyPart.GetComponent<CharacterJoint>();
        if (j)
        {
            j.connectedBody = GetComponent<Rigidbody>();
        }
        else
        {
            joint.connectedBody = bodyPart.gameObject.GetComponent<Rigidbody>();
            //var newJoint = bodyPart.gameObject.AddComponent<CharacterJoint>();
            //newJoint.
        }
        

    }

    void IgnoreAllCollisions(GameObject obj)
    {
        foreach (var playerCollider in GetComponentsInParent<Collider>())
        {
            foreach (var bodyPartCollider in obj.GetComponents<Collider>())
            {
                Physics.IgnoreCollision(playerCollider, bodyPartCollider, true);
            }
        }
    }

    void AdjustPosition(BodyPart bodyPart)
    {
        var hardPoint = bodyPart.GetHardPoint();
        if (!hardPoint)
        {
            Debug.LogError("BodyPart is missing HardPoint! Attaching to center of mass");
            hardPoint = bodyPart.gameObject;
        }
        // Adjust position in such way that the hard point is inside the BodyPartConnector
        bodyPart.transform.rotation = transform.rotation;
        bodyPart.transform.position += transform.position - hardPoint.transform.position;
        
    }

    public void Detach()
    {
        if (attachedPart)
            attachedPart.SetAttached(false);
        attachedPart = null;
        joint.connectedBody = null;
    }
}

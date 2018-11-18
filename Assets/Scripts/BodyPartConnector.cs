using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartConnector : MonoBehaviour
{
    public GameObject attachedPart;

    private CharacterJoint joint;
    private Body body;

	// Use this for initialization
	void Start ()
	{
	    joint = GetComponent<CharacterJoint>();
	    body = GetComponent<Body>();
	    if (!body)
	    {
	        body = GetComponentInParent<Body>();
	    }

	    if (!body)
	    {
	        body = transform.parent.GetComponentInChildren<Body>();
	    }

        if (!body)
	    {
            Debug.LogError("Cannot find body for BodyPartConnector!");
	    }

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool IsEmpty()
    {
        return attachedPart == null;
    }

    public void AttachTo(BodyPart bodyPart)
    {
        IgnoreAllCollisions(bodyPart.gameObject);
        //AdjustPosition(bodyPart);
        attachedPart = bodyPart.gameObject;
        bodyPart.SetAttached(true);
        var hardPoint = bodyPart.GetHardPoint();
        Vector3 pos = bodyPart.transform.position - hardPoint.transform.position;
        bodyPart.transform.parent = transform;
        /*
        Vector3 scale = bodyPart.transform.localScale;
        scale.x /= transform.localScale.x;
        scale.y /= transform.localScale.y;
        scale.z /= transform.localScale.z;
        bodyPart.transform.localScale = scale;
        */
        bodyPart.transform.localRotation = Quaternion.identity;
        ConnectJoint(bodyPart.gameObject.GetComponent<Rigidbody>(), hardPoint.transform.localPosition);
    }

    public void AttachTo(Body otherBody)
    {
        /*
        IgnoreAllCollisions(otherBody.gameObject);
        
        if (!connector)
            return;
        //AdjustPosition(connector.gameObject);
        attachedPart = otherBody.gameObject;
        
        connector.attachedPart = gameObject;
        otherBody.attachedToPlayer = true;
        Vector3 pos = otherBody.transform.position - connector.transform.position;
        ConnectJoint(otherBody.gameObject.GetComponent<Rigidbody>(), pos);
        */
        attachedPart = otherBody.gameObject;
        var connector = otherBody.GetClosestEmptyConnector(transform.position);
        var rb = otherBody.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
        }

        otherBody.transform.parent = transform;
        if (connector)
        {
            otherBody.transform.localPosition = -connector.transform.localPosition;
        }
        else
        {
            otherBody.transform.localPosition = Vector3.zero;
        }
        otherBody.transform.localRotation = Quaternion.identity;

    }

    void IgnoreAllCollisions(GameObject obj)
    {
        var parent = obj.transform;
        while (parent.parent)
        {
            parent = parent.parent;
        }
        IgnoreAllChildCollisions(parent.gameObject);
        //parent.tag = "Player";
        //parent.gameObject.layer = LayerMask.NameToLayer("Player");
        /*
        foreach (var playerCollider in GetComponentsInParent<Collider>())
        {
            foreach (var bodyPartCollider in obj.GetComponents<Collider>())
            {
                Physics.IgnoreCollision(playerCollider, bodyPartCollider, true);
            }
        }
        */
    }

    void ConnectJoint(Rigidbody rigidbody, Vector3 connectedObjectPosition)
    {
        joint.connectedBody = rigidbody;
        joint.anchor = Vector3.zero;
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = connectedObjectPosition;
        /*
        var newJoint = body.gameObject.AddComponent<CharacterJoint>();
        newJoint.autoConfigureConnectedAnchor = false;
        newJoint.anchor = body.transform.position - transform.position;
        newJoint.connectedBody = rigidbody;
        newJoint.connectedAnchor = connectedObjectPosition;
        */
    }

    void IgnoreAllChildCollisions(GameObject obj)
    {
        obj.layer = LayerMask.NameToLayer("Player");
        foreach (Transform child in obj.transform)
        {
            IgnoreAllChildCollisions(child.gameObject);
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
        bodyPart.transform.rotation = transform.rotation;
        bodyPart.transform.position += transform.position - hardPoint.transform.position;

    }

    /*void AdjustPosition(GameObject obj)
    {
        

    }
    */
    /*
    public void Detach()
    {
        if (attachedPart)
            attachedPart.SetAttached(false);
        attachedPart = null;
        joint.connectedBody = null;
    }
    */
}

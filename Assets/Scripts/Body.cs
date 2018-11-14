using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    public List<BodyPartConnector> BodyPartConnectors;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        BodyPart bodyPart = collision.gameObject.GetComponent<BodyPart>();
        if (!bodyPart)
            return;
        BodyPartConnector closestConnector = GetClosestEmptyConnector(collision.transform.position);
        if (!closestConnector)
        {
            Debug.Log("No empty connectors found!");
            return;
        }
        Debug.Log("Attaching bodypart with id " + bodyPart.GetID().ToString());
        closestConnector.AttachTo(bodyPart);
    }



    BodyPartConnector GetClosestEmptyConnector(Vector3 position)
    {
        BodyPartConnector closest = null;
        float minDistance = 9999;
        foreach (var connector in BodyPartConnectors)
        {
            if (connector.IsEmpty())
                continue;

            float distance = Vector3.Distance(connector.transform.position, position);
            if (!closest || distance < minDistance)
            {
                minDistance = distance;
                closest = connector;
            }
                
        }
        return closest;
    }
}

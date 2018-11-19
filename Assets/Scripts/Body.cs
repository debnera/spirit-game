using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class Body : MonoBehaviour
{
    public List<BodyPartConnector> BodyPartConnectors;
    public bool attachedToPlayer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool IsAttachedToPlayer()
    {
        return attachedToPlayer;
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision);
    }

    public void HandleCollision(Collision collision)
    {
        BodyPartConnector closestConnector = GetClosestEmptyConnector(collision.contacts[0].point);
        if (!closestConnector)
        {
            Debug.Log("No empty connectors found!");
            return;
        }
        BodyPart bodyPart = collision.gameObject.GetComponent<BodyPart>();
        Body body = collision.gameObject.GetComponent<Body>();

        if (bodyPart && !bodyPart.IsAttached())
        {
            closestConnector.AttachTo(bodyPart);
            OnAttach();
        }
        else if (body && !body.IsAttachedToPlayer())
        {
            closestConnector.AttachTo(body);
            OnAttach();
        }
    }

    public BodyPartConnector GetClosestEmptyConnector(Vector3 position)
    {
        BodyPartConnector closest = null;
        float minDistance = 9999;
        foreach (var connector in BodyPartConnectors)
        {
            if (!connector.IsEmpty())
                continue;

            float distance = Vector3.Distance(connector.transform.position, position);
            Debug.Log(distance);
            if (!closest || distance < minDistance)
            {
                minDistance = distance;
                closest = connector;
            }
                
        }
        Debug.Log("Mindistance: " + minDistance);
        return closest;
    }

    void OnAttach()
    {
        int connected = 0;
        foreach (var connector in BodyPartConnectors)
        {
            if (!connector.IsEmpty())
                connected++;
        }
        Debug.Log(connected);
        if (connected > 3)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController)
            {
                playerController.WalkingMode = true;
            }
        }
    }

    public void FreezeToStatue()
    {
        Freeze(gameObject);
        foreach (var connector in BodyPartConnectors)
        {
            Freeze(connector.attachedPart);
        }
    }

    private void Freeze(GameObject obj)
    {
        if (!obj)
            return;
        var rbody = obj.GetComponent<Rigidbody>();
        if (rbody)
        {
            rbody.isKinematic = false;
            rbody.detectCollisions = false;
        }
    }

    public void Save(String path, String filename)
    {
        String fullpath = path + Path.DirectorySeparatorChar + filename;
        Directory.CreateDirectory(path);
        FileStream fs = new FileStream(fullpath, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(fs, ToSerializableList());
            Debug.Log("Statue saved to " + path);
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

    public bool Load(String path)
    {
        if (!File.Exists(path))
        {
            Debug.Log("Path does not exist: " + path);
            return false;
        }

        List<SerializableBodyPart> bodyParts = new List<SerializableBodyPart>();
        FileStream fs = new FileStream(path, FileMode.Open);
        try
        {
            Debug.Log("Loading " + path);
            BinaryFormatter formatter = new BinaryFormatter();
            bodyParts = (List<SerializableBodyPart>)formatter.Deserialize(fs);
            FromSerializableList(bodyParts);

        }
        catch (Exception e)
        {
            Debug.Log("Loading failed: " + e.Message);
            return false;
        }
        finally
        {
            fs.Close();
        }

        

        return true;
    }

    List<SerializableBodyPart> ToSerializableList()
    {
        List<SerializableBodyPart> bodyparts = new List<SerializableBodyPart>();
        foreach (var connector in BodyPartConnectors)
        {
            if (!connector.attachedPart || !connector.attachedPart.GetComponent<BodyPart>())
                bodyparts.Add(null);
            else
            {
                BodyPart part = connector.attachedPart.GetComponent<BodyPart>();
                SerializableBodyPart serializablePart = SerializableBodyPart.FromBodyPart(part);
                bodyparts.Add(serializablePart);
            }

        }
        return bodyparts;
    }

    void FromSerializableList(List<SerializableBodyPart> serializableBodyParts)
    {
        for (int i = 0; i < serializableBodyParts.Count; i++)
        {
            if (serializableBodyParts[i] != null)
            {
                BodyPartConnector connector = BodyPartConnectors[i];
                BodyPart bodyPart = serializableBodyParts[i].ToBodyPart(connector.transform);
                connector.attachedPart = bodyPart.gameObject;
            }
        }
    }
}

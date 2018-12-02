﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class Body : MonoBehaviour
{
    public List<BodyPartConnector> bodyPartConnectors;
    public BodyPartConnector[] legConnectors = new BodyPartConnector[2];
    public bool attachedToPlayer;
    private float scale = 1;
    private Vector3 originalScale;
    private Animator movementAnimator;
    private Dictionary<CharacterJoint, Vector3> connectorAnchorPositions = new Dictionary<CharacterJoint, Vector3>();

	// Use this for initialization
    void Awake()
    {
        originalScale = transform.localScale;
        movementAnimator = GetComponent<Animator>();
        EnableMovementAnimation(false);
    }

    void Start()
    {
        foreach (var connector in bodyPartConnectors)
        {
            CharacterJoint joint = connector.GetComponent<CharacterJoint>();
            if (joint)
                connectorAnchorPositions.Add(joint, joint.connectedAnchor);
        }
    }

    void ScaleConnectorAnchorPositions(float scale)
    {
        foreach (KeyValuePair<CharacterJoint, Vector3> jointPosPair in connectorAnchorPositions)
        {
            jointPosPair.Key.connectedAnchor = jointPosPair.Value * scale;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public int GetNumberOfConnectedLimbs()
    {
        int amount = 0;
        foreach (var bodyPartConnector in bodyPartConnectors)
        {
            if (bodyPartConnector.attachedPart) amount++;
        }

        return amount;
    }

    public float GetMaxLegHeight()
    {
        float maxHeight = 0;
        foreach (var bodyPartConnector in legConnectors)
        {
            if (bodyPartConnector.attachedPart)
            {
                BodyPart bodyPart = bodyPartConnector.attachedPart.GetComponent<BodyPart>();
                if (bodyPart)
                {
                    maxHeight = Mathf.Max(bodyPart.GetHeight(), maxHeight);
                }
            }
        }
        return maxHeight;
    }

    public void EnableMovementAnimation(bool value)
    {
        if (!movementAnimator) return;
        movementAnimator.enabled = value;
    }

    public void SetMovementAnimationSpeed(float speed)
    {
        if (!movementAnimator) return;
        movementAnimator.speed = speed;
    }

    public bool IsAttachedToPlayer()
    {
        return attachedToPlayer;
    }

    public void SetScale(float newScale)
    {
        scale = newScale;
        transform.localScale = originalScale * scale;
        ScaleConnectorAnchorPositions(scale);
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
            //Debug.Log("No empty connectors found!");
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
        foreach (var connector in bodyPartConnectors)
        {
            if (!connector.IsEmpty())
                continue;

            float distance = Vector3.Distance(connector.transform.position, position);
            //Debug.Log(distance);
            if (!closest || distance < minDistance)
            {
                minDistance = distance;
                closest = connector;
            }
                
        }
        //Debug.Log("Mindistance: " + minDistance);
        return closest;
    }

    void OnAttach()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        int connected = 0;
        foreach (var connector in bodyPartConnectors)
        {
            if (!connector.IsEmpty())
                connected++;
        }
        Debug.Log(connected);
        if (connected > 3)
        {
            if (playerController)
            {
                playerController.WalkingMode = true;
            }
        }

        if (playerController)
        {
            playerController.OnBodyPartAttach();
        }
    }

    public void FreezeToStatue()
    {
        RecursiveFreeze(transform.root.gameObject);
        enabled = false;
    }

    private void RecursiveFreeze(GameObject obj)
    {
        Freeze(obj);
        foreach (Transform child in obj.transform)
        {
            RecursiveFreeze(child.gameObject);
        }
    }

    private void Freeze(GameObject obj)
    {
        if (!obj)
            return;
        obj.layer = LayerMask.NameToLayer("Default");
        var rbody = obj.GetComponent<Rigidbody>();
        if (rbody)
        {
            rbody.isKinematic = true;
            rbody.detectCollisions = true;
        }
    }

    public void Save(String path, String filename)
    {
        String fullpath = path + Path.DirectorySeparatorChar + filename;
        Directory.CreateDirectory(path);
        FileStream fs = new FileStream(fullpath, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        SerializableVector3 position = transform.position;
        SerializableVector3 rotation = transform.eulerAngles;
        try
        {
            formatter.Serialize(fs, ToSerializableList());
            formatter.Serialize(fs, position);
            formatter.Serialize(fs, rotation);
            formatter.Serialize(fs, scale);
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
            SerializableVector3 position = (SerializableVector3)formatter.Deserialize(fs);
            SerializableVector3 rotation = (SerializableVector3)formatter.Deserialize(fs);
            scale = (float)formatter.Deserialize(fs);
            FromSerializableList(bodyParts);
            transform.position = position;
            transform.eulerAngles = rotation;
            SetScale(scale);
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
        foreach (var connector in bodyPartConnectors)
        {
            if (!connector.attachedPart || !connector.attachedPart.GetComponent<BodyPart>())
                bodyparts.Add(null);
            else
            {
                BodyPart part = connector.attachedPart.GetComponent<BodyPart>();
                SerializableBodyPart serializablePart = new SerializableBodyPart(connector.transform.localPosition, connector.transform.localEulerAngles, part.GetID());
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
                BodyPartConnector connector = bodyPartConnectors[i];
                SerializableBodyPart serializableBodyPart = serializableBodyParts[i];
                BodyPart bodyPart = serializableBodyPart.ToBodyPart();
                connector.AttachTo(bodyPart);
                connector.transform.localPosition = serializableBodyPart.position;
                connector.transform.eulerAngles = serializableBodyPart.rotation;
            }
        }
    }
}

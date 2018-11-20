using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableBodyPart
{
    public SerializableVector3 position;
    public SerializableVector3 rotation;
    public int id;

    public SerializableBodyPart(Vector3 position, Vector3 rotation, int id)
    {
        this.position = position;
        this.rotation = rotation;
        this.id = id;
    }

    static public BodyPart ToBodyPart(SerializableBodyPart serializableBodyPart)
    {
        BodyPartRegistry registry = BodyPartRegistry.GetInstance();
        if (!registry)
        {
            return null;
        }
        try
        {
            BodyPart newBodyPart = GameObject.Instantiate(registry.GetBodyPart(serializableBodyPart.id));
            return newBodyPart;
        }
        catch (Exception e)
        {
            Debug.Log("Could not instantiate a new BodyPart with id: " + serializableBodyPart.id + "\nError: " + e.Message);
            return null;
        }
    }

    public BodyPart ToBodyPart()
    {
        return ToBodyPart(this);
    }
}

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

    static public SerializableBodyPart FromBodyPart(BodyPart BodyPart)
    {
        return new SerializableBodyPart(BodyPart.transform.localPosition, BodyPart.transform.localEulerAngles, BodyPart.GetID());
    }

    static public BodyPart ToBodyPart(SerializableBodyPart serializableBodyPart, Transform parent)
    {
        //BodyPart newBodyPart = Instant
        BodyPartRegistry registry = BodyPartRegistry.GetInstance();
        if (!registry)
        {
            return new BodyPart();
        }

        try
        {
            BodyPart newBodyPart = GameObject.Instantiate(registry.GetBodyPart(serializableBodyPart.id), parent);
            newBodyPart.transform.localPosition = serializableBodyPart.position;
            newBodyPart.transform.localEulerAngles = serializableBodyPart.rotation;
            return newBodyPart;
        }
        catch (Exception e)
        {
            Debug.Log("Could not instantiate a new BodyPart with id: " + serializableBodyPart.id + "\nError: " + e.Message);
        }
        return new BodyPart();
    }

    public BodyPart ToBodyPart(Transform parent)
    {
        return ToBodyPart(this, parent);
    }
}

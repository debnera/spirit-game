using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableBodyPart : MonoBehaviour
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

    public SerializableBodyPart FromBodyPart(BodyPart BodyPart)
    {
        return new SerializableBodyPart(BodyPart.transform.localPosition, BodyPart.transform.localEulerAngles, BodyPart.GetID());
    }

    public BodyPart ToBodyPart(SerializableBodyPart serializableBodyPart, Transform parent)
    {
        //BodyPart newBodyPart = Instant
        BodyPartRegistry registry = BodyPartRegistry.GetInstance();
        if (!registry)
        {
            return new BodyPart();
        }
        BodyPart newBodyPart = Instantiate(registry.GetBodyPart(id), parent);
        newBodyPart.transform.localPosition = position;
        newBodyPart.transform.localEulerAngles = rotation;
        return newBodyPart;
    }
}

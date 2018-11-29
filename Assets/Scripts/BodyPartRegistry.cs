using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartRegistry : MonoBehaviour
{

    [Serializable]
    public struct BodyPartID
    {
        public int id;
        public BodyPart bodyPart;
    }

    public BodyPartID[] BodyParts;

    void Awake()
    {
        TestIDs();
    }

    public void TestIDs()
    {
        foreach (var keyValuePair in BodyParts)
        {
            if (keyValuePair.id != keyValuePair.bodyPart.GetID())
            {
                Debug.LogError("BodyPart id does not match in BodyPartRegistry! (" +
                               keyValuePair.id + " != " + keyValuePair.bodyPart.GetID());
            }
        }
    }


    /*
    public int GetID(BodyPart BodyPart)
    {
        return BodyParts.FindIndex(obj => obj == BodyPart);
    }
    */

    public BodyPart GetBodyPart(int id)
    {
        foreach (var keyValuePair in BodyParts)
        {
            if (keyValuePair.id == id)
            {
                return keyValuePair.bodyPart;
            }
        }

        return null;
    }

    public static BodyPartRegistry GetInstance()
    {
        BodyPartRegistry registry = FindObjectOfType<BodyPartRegistry>();
        if (!registry)
        {
            Debug.LogError("Cannot find the BodyPartRegistry! Make sure the scene has an object of type BodyPartRegistry.");
        }

        return registry;
    }
}

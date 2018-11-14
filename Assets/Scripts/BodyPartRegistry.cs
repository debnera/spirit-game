using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class BodyPartRegistry : MonoBehaviour
{

    public List<BodyPart> BodyParts;

    public int GetID(BodyPart BodyPart)
    {
        return BodyParts.FindIndex(obj => obj == BodyPart);
    }

    public BodyPart GetBodyPart(int index)
    {
        return BodyParts[index];
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BodyPart : MonoBehaviour
{
    public GameObject hardPoint;
    //private BodyPartRegistry registry;
    public int ID = -1;
    private bool attached = false;
    private BodyPart parent;
    private float height;

    void Start()
    {
        /*
        registry = BodyPartRegistry.GetInstance();
        if (registry)
            ID = registry.GetID(this);
        */
        CalculateHeight();
    }

    void CalculateHeight()
    {
        SkinnedMeshRenderer mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        if (mesh)
        {
            // Guess that the height is the length of the longest side of the bounding box
            float maxVal = 0;
            for (int i = 0; i < 3; i++)
            {
                maxVal = Mathf.Max(maxVal, mesh.bounds.extents[i]);
            }
            height = maxVal * 2;
        }
    }

    public float GetHeight()
    {
        return height;
    }

    public int GetID()
    {
        return ID;
    }

    public bool IsAttached()
    {
        return attached;
    }

    public void SetAttached(bool value)
    {
        attached = value;
    }

    public void AttachTo(GameObject newParent)
    {
        if (!attached)
        {
            transform.parent = newParent.transform;
            attached = true;
        }
            
    }

    public GameObject GetHardPoint()
    {
        return hardPoint;
    }
}

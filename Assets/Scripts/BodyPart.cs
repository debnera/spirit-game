using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BodyPart : MonoBehaviour
{

    public GameObject hardPoint;
    private BodyPartRegistry registry;
    private int ID = -1;
    private bool attached = false;

    void Start()
    {
        registry = BodyPartRegistry.GetInstance();
        if (registry)
            ID = registry.GetID(this);
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

    public void AttachTo(GameObject parent)
    {
        if (!attached)
        {
            transform.parent = parent.transform;
            attached = true;
        }
            
    }

    public GameObject GetHardPoint()
    {
        return hardPoint;
    }
}

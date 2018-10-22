using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : MonoBehaviour
{
    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        triggered = true;
        var rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
        {
            rigidBody.useGravity = true;
        }
        Destroy(this, 20f);
    }
}

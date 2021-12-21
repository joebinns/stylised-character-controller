using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidParent : MonoBehaviour
{
    public Rigidbody targetRB;

    void Start()
    {
        if (targetRB != null)
        {
            transform.position = targetRB.transform.position;
            transform.rotation = targetRB.transform.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (targetRB != null)
        {
            transform.position = targetRB.transform.position;
            transform.rotation = targetRB.transform.rotation;
        }
    }
}

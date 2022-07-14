using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Acts to mirror a rigidbody's position and rotation.
/// This is intended to be used to avoid cases in which a rigidbody parent has a rigidbody child, avoiding strange results.
/// </summary>
public class RigidParent : MonoBehaviour
{
    public Rigidbody targetRB;

    /// <summary>
    /// Set the transform position and rotation to match the targetRB's.
    /// </summary>
    void Start()
    {
        if (targetRB != null)
        {
            transform.position = targetRB.transform.position;
            transform.rotation = targetRB.transform.rotation;
        }
    }

    /// <summary>
    /// Update the transform position and rotation to match the targetRB's.
    /// </summary>
    private void FixedUpdate()
    {
        if (targetRB != null)
        {
            transform.position = targetRB.transform.position;
            transform.rotation = targetRB.transform.rotation;
        }
    }
}

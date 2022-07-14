using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Limits the range of rotation of this rigid body.
/// </summary>
public class LimitRotation : MonoBehaviour
{
    // +- Range of rotations for each respective axis.
    [SerializeField] private Vector3 maxLocalRotation = Vector3.one * 360f;

    private Rigidbody _rb;

    /// <summary>
    /// Define the rigid body.
    /// </summary>
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Clamp the rotation to be less than the desired maxLocalRotation.
    /// </summary>
    private void FixedUpdate()
    {
        Quaternion clampedLocalRot = MathsUtils.ClampRotation(transform.localRotation, maxLocalRotation);
        _rb.MoveRotation(clampedLocalRot);
    }
}

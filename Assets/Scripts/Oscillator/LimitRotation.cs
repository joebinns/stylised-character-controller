using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitRotation : MonoBehaviour
{
    // +- Range of rotations for each respective axis.
    [SerializeField] private Vector3 maxLocalRotation = Vector3.one * 360f;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Quaternion clampedLocalRot = MathsUtils.ClampRotation(transform.localRotation, maxLocalRotation);
        _rb.MoveRotation(clampedLocalRot);
    }
}

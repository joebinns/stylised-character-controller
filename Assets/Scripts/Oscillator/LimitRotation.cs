using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitRotation : MonoBehaviour
{
    // +- Range of rotations for each respective axis.
    [SerializeField] private Vector3 maxRotation = Vector3.one * 360f;

    private Vector3 _prevLocalPosition;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Quaternion clampedRot = MathsUtils.ClampRotation(transform.localRotation, maxRotation);
        _rb.MoveRotation(clampedRot);
    }
}

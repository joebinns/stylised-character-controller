using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Linearly interpolate the camera position over time to follow a series of pendulums.
/// </summary>
public class LerpCamera : MonoBehaviour
{
    [SerializeField] private PendulumCreator pendulumCreator;

    private Vector3 start;
    private Vector3 end;
    private float duration;

    private float t;

    private Vector3 originalPos;

    /// <summary>
    /// Declare the original position of the camera.
    /// </summary>
    private void Start()
    {
        originalPos = transform.localPosition;
    }

    /// <summary>
    /// Lerp the camera, if the series of pendulums has begun creation.
    /// </summary>
    void Update()
    {
        if (pendulumCreator.started == true)
        {
            start = Vector3.zero;
            end = pendulumCreator.displacement - pendulumCreator.popInDistance * Vector3.left - pendulumCreator.transform.position;
            duration = pendulumCreator.numPendulums * pendulumCreator.incrementalTime;

            t += Time.deltaTime / duration;

            transform.localPosition = originalPos + MathsUtils.LerpVector3(start, end, t);
        }
    }
}

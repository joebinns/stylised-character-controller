using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpCamera : MonoBehaviour
{
    [SerializeField] private PendulumCreator pendulumCreator;

    private Vector3 start;
    private Vector3 end;
    private float duration;

    private float t;

    private Vector3 originalPos;

    private void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        start = Vector3.zero;
        end = pendulumCreator.displacement - pendulumCreator.popInDistance * Vector3.back - pendulumCreator.transform.position;
        duration = pendulumCreator.numPendulums * pendulumCreator.incrementalTime;

        t += Time.deltaTime / duration;
        
        transform.localPosition = originalPos + MathsUtils.LerpVector3(start, end, t);
    }
}

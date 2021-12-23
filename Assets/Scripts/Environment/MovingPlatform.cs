using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private float _t = 0f;
    private Oscillator _oscillator;

    [SerializeField] private float radiusOfRotation = 20f;
    [SerializeField] private float rateOfRotation = 1f;

    private void Start()
    {
        _oscillator = GetComponent<Oscillator>();

        _oscillator.transform.position = new Vector3(radiusOfRotation * -Mathf.Cos(0f), 1f, radiusOfRotation * Mathf.Sin(0f));
    }

    private void FixedUpdate()
    {
        _t += Time.fixedDeltaTime;
        float theta = _t * Mathf.PI / 180f * rateOfRotation;
        _oscillator.localEquilibriumPosition = new Vector3(radiusOfRotation * -Mathf.Cos(theta), 1f, radiusOfRotation * Mathf.Sin(theta));


        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(0f, Time.fixedDeltaTime * rateOfRotation, 0f));
    }
}

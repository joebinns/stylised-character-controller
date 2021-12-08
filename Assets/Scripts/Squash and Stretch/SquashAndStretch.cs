using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    private Vector3 _restScale;

    void Start()
    {
        _restScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        Vector3 newScale = Vector3.zero;

        newScale += GetScaleFromOscillator();
        newScale = MaintainVolume(newScale); // i.e. If squashing in one axis, stretch in the other axes.

        newScale += _restScale;
        ApplyScale(newScale);
    }

    private Vector3 MaintainVolume(Vector3 vec)
    {
        Vector3 newScale = vec;

        if (newScale != Vector3.zero)
        {
            Vector3 rightCross = Vector3.Cross(vec, transform.right);
            Vector3 upCross = Vector3.Cross(vec, transform.up);
            Vector3 forwardCross = Vector3.Cross(vec, transform.forward);

            // Apply counter scaling to cross products.
            newScale += rightCross / 2f;
            newScale += upCross / 2f;
            newScale += forwardCross / 2f;
        }

        return newScale;
    }

    public Oscillator dampenedOscillator;
    private Vector3 GetScaleFromOscillator()
    {
        Vector3 newScale = - dampenedOscillator.transform.localPosition * 0.15f;

        Vector3 scaleCont = newScale;
        return scaleCont;
    }

    private void ApplyScale(Vector3 newScale)
    {
        transform.localScale = newScale;
    }
}

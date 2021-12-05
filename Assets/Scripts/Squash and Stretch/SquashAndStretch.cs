using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 restScale;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        restScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        Vector3 newScale = Vector3.zero;

        newScale += GetScaleFromOscillator();
        newScale = MaintainVolume(newScale); // i.e. If squashing in one axis, stretch in the other axes.

        newScale += restScale;
        ApplyScale(newScale);

        //Debug.Log(newScale.magnitude);
    }

    private Vector3 MaintainVolume(Vector3 vec)
    {
        Vector3 newScale = vec;

        if (vec != Vector3.zero)
        {
            Vector3 rightCross = Vector3.Cross(vec, transform.right);
            Vector3 upCross = Vector3.Cross(vec, transform.up);
            Vector3 forwardCross = Vector3.Cross(vec, transform.forward);

            //Debug.Log("==============");
            //Debug.Log("TRANSFORM UP:" + transform.up);
            //Debug.Log("VECTOR:" + vec);
            //Debug.Log("RIGHT CROSS:" + rightCross);
            //Debug.Log("UP CROSS:" + upCross);
            //Debug.Log("FORWARD CROSS:" + forwardCross);

            // Apply counter scaling to cross products.
            newScale += rightCross / 2f;
            newScale += upCross / 2f;
            newScale += forwardCross / 2f;

            //Debug.Log(newScale);

        }

        return newScale;
    }

    public DampenedOscillator dampenedOscillator;
    private Vector3 GetScaleFromOscillator()
    {
        Vector3 newScale = - dampenedOscillator.transform.localPosition * 0.15f;

        //newScale = transform.InverseTransformDirection(newScale);

        Vector3 scaleCont = newScale;
        return scaleCont;
    }

    private void ApplyScale(Vector3 newScale)
    {
        transform.localScale = newScale;
    }
}

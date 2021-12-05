using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DampenedOscillator : MonoBehaviour
{
    //public static DampenedOscillator Instance { get; private set; }

    //private SquashAndStretch sas;

    public Vector3 centreDisplacement = Vector3.zero;

    private void Awake()
    {
        //Instance = this;

        //parentRB = this.GetComponentInParent<Rigidbody>();

        //transform.position = transform.parent.localPosition;

        //sas = parentRB.GetComponent<SquashAndStretch>();
    }

    //private float prevX = 0f; // Previous position of the object.
    private Vector3 prevX = Vector3.zero;

    [HideInInspector] public Vector3 velocity = Vector3.zero;

    [SerializeField] private float k = 100f; // Spring strength constant.
    [SerializeField] private float b = 2f; // Spring damper constant.

    private void FixedUpdate()
    {
        // DANGER: NOT SURE IF I HAVE DEALT WITH TRANSFORM DIRECTIONS PROPERLY.... 
        //AddForce(transform.InverseTransformDirection(parentRB.GetComponent<SquashAndStretch>().acc)/2f);
        //Debug.Log(transform.InverseTransformDirection(parentRB.GetComponent<SquashAndStretch>().acc)/2f);
        //Debug.Log(Vector3.Magnitude(transform.InverseTransformDirection(parentRB.GetComponent<SquashAndStretch>().acc)/2f));
        //sas.

        //float x = transform.localPosition.y; // Displacement.
        Vector3 x = transform.localPosition - centreDisplacement;

        Vector3 v = (x - prevX) / Time.fixedDeltaTime; // Velocity.
        velocity = v;
        prevX = x;

        Vector3 springForce = (k * x) + (b * v); // Dampened spring force.
        springForce = -springForce; // Set '-', since the force is attractive.

        //Vector3 springForceVec = new Vector3(0f, springForce, 0f);
        Vector3 springForceVec = new Vector3(springForce.x, springForce.y, springForce.z);

        AddForce(springForceVec);

        //Debug.Log("x:" + x);
        //Debug.Log("v_x:" + v_x);
        //Debug.Log("spring:" + springForceVec);
    }

    private Vector3 prevV = Vector3.zero; // Previous calculated velocity for the force being applied (not the velocity of the object).
    public void AddForce(Vector3 F)
    {
        Vector3 a = F; // Acceleration. No rb, so no mass to easily call.
        Vector3 v = a * Time.fixedDeltaTime + prevV; // Velocity.
        prevV = v;
        Vector3 x = v * Time.fixedDeltaTime; // Displacement.

        transform.localPosition += x;
        //transform.position += x;
    }
}

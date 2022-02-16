using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraBouncy : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] private float _extraBounceMultiplier = 10f;

    [SerializeField] private bool _shouldBounceBack = true;

    void Start()
    {
        _rb = this.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        ExtraBounce(collision);
    }

    [SerializeField] private Oscillator _optionalOscillator;
    private void ExtraBounce(Collision collision)
    {
        Vector3 impulse = collision.impulse;

        float minImp = Mathf.Log(2f);
        float imp = Mathf.Log(impulse.magnitude);

        Vector3 force;

        imp = Mathf.Clamp(imp, minImp, Mathf.Infinity);
        force = collision.GetContact(0).normal * imp / Time.fixedDeltaTime;

        Vector3 extraBounceForce = force * _extraBounceMultiplier; // * collision.gameObject.GetComponent<Collider>().material.bounciness;

        // _rb.AddForce(extraBounceForce);
        _rb.AddForceAtPosition(extraBounceForce, collision.GetContact(0).point);
        if (_shouldBounceBack)
        {
            try
            {
                collision.rigidbody.AddForce(-extraBounceForce);
            }
            catch
            {

            }
        }


        if (_optionalOscillator != null)
        {
            // Squash and Stretch stuff.
            Vector3 oscillatorForce = _optionalOscillator.transform.InverseTransformDirection(extraBounceForce);
            for (int i = 0; i < 3; i++)
            {
                // Make the extraBounceForce applied to the oscillator in the negative direction (should compress first).
                if (oscillatorForce[i] < 0)
                {
                    oscillatorForce[i] *= -1;
                }
            }
            _optionalOscillator.ApplyForce(oscillatorForce);
        }

    }
}

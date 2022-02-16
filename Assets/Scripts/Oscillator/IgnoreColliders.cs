using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreColliders : MonoBehaviour
{
    [SerializeField] private List<Collider> collidersToIgnore;
    private Collider thisCollider;

    void Start()
    {
        thisCollider = this.GetComponent<Collider>();

        foreach (Collider otherCollider in collidersToIgnore)
        {
            Physics.IgnoreCollision(thisCollider, otherCollider);
        }
    }
}

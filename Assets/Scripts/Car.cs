using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    //void OnCollisionEnter(Collision col)
    //{
    //    var contact = col.GetContact(0).point;
    //    col.rigidbody.AddExplosionForce(100, contact, 1, 20);
    //}

    void OnTriggerEnter(Collider other)
    {
        // ignore weapons getting pushed
        if (other.TryGetComponent<Weapon>(out _))
            return;

        if (other.name.Contains("Rock", StringComparison.OrdinalIgnoreCase))
            other.attachedRigidbody.AddExplosionForce(5000f, transform.position, 50f);
        VFXManager.Instance.apply_force(transform.position, 3000f, 7f);
        CameraManager.Instance.DoShake(0.3f, 2f, 0.7f);
    }
}

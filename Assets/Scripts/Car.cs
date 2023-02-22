using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        var ignore = new List<Rigidbody>();
        foreach (var weapon in FindObjectsOfType<Weapon>())
        {
            if (weapon.TryGetComponent<Rigidbody>(out var rb))
                ignore.Add(rb);
        }

        VFXManager.Instance.apply_force(transform.position, 3000f, 7f, ignore.ToArray());
        CameraManager.Instance.DoShake(0.3f, 16f);
    }
}

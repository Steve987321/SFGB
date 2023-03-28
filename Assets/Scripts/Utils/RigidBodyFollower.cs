using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// dont use
/// </summary>
public class RigidBodyFollower : MonoBehaviour
{
    public Transform Target;

    public Vector3 RotationOffset;

    public void SetTarget(Transform target)
    {
        Target = target;
    }
    public void SetTarget(Transform target, Vector3 rotoffset)
    {
        Target = target;
        RotationOffset = rotoffset;
    }

    void FixedUpdate()
    {
        var rb = GetComponent<Rigidbody>();
        if (Target == null) return;

        if (RotationOffset == Vector3.zero)
        {
            rb.MovePosition(Target.position);
            rb.MoveRotation(Target.rotation);
        }
        else
        {
            rb.MovePosition(Target.position);
            rb.MoveRotation(Target.rotation * Quaternion.Euler(RotationOffset));
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour
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

    void Update()
    {
        if (Target == null) return;

        if (RotationOffset != Vector3.zero)
            transform.SetPositionAndRotation(Target.position, Target.rotation * Quaternion.Euler(RotationOffset));
        else
            transform.SetPositionAndRotation(Target.position, Target.rotation);

    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimCopy : MonoBehaviour
{
    [SerializeField] private Transform AnimTargetLimb;
    private Quaternion _initialRot;

    void Start()
    {
        _initialRot = AnimTargetLimb.transform.localRotation;
    }

    void FixedUpdate()
    {
        GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Inverse(this.AnimTargetLimb.localRotation) * _initialRot;
    }
}

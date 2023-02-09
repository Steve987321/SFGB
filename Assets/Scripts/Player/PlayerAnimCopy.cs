using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimCopy : MonoBehaviour
{
    [SerializeField] private Transform AnimTargetLimb;
    private Quaternion initialRot;
    
    void Start()
    {
        initialRot = AnimTargetLimb.transform.localRotation;
    }

    void FixedUpdate()
    {
        GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Inverse(this.AnimTargetLimb.localRotation) * initialRot;
    }
}

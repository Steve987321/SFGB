using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RelativePlayerRotation : NetworkBehaviour
{
    public float RotationSpeed = 10;

    [SerializeField] private LayerMask _ignoreLayer;
    [SerializeField] private Transform _rbObj;

    // old 
    //private void LateUpdate()
    //{
    //    // Get the direction of where the player should ahead in world space
    //    var hMoveDir = Camera.main.transform.right * Input.GetAxis("Horizontal");
    //    var vMoveDir = Camera.main.transform.forward * Input.GetAxis("Vertical");
    //    var moveDir = hMoveDir + vMoveDir;

    //    if (!(moveDir.magnitude > 0)) return;

    //    // Create the rotation we need according to moveDir
    //    var lookRotation = Quaternion.LookRotation(moveDir);
    //    lookRotation.x = 0;
    //    lookRotation.z = 0;

    //    // Rotate player over time according to speed until we are in the required rotation
    //    this.transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);

    //}

    void LateUpdate()
    {
        if (!IsOwner) return;

        //var pos = Camera.main.ScreenToWorldPoint(new Vectoasr3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 10);
        if (Physics.Raycast(ray, out var hit, 100f, 1 << _ignoreLayer.value))
        {
            Debug.DrawLine(hit.point, _rbObj.position);
            var target = Quaternion.LookRotation(hit.point - _rbObj.position);
            target.z = 0;
            target.x = 0;
            transform.rotation = target;
        }
    }
}

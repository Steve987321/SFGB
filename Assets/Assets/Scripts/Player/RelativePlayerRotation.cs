using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativePlayerRotation : MonoBehaviour
{
    public float RotationSpeed = 10;

    private void LateUpdate()
    {
        // Get the direction of where the player should ahead in world space
        var hMoveDir = Camera.main.transform.right * Input.GetAxis("Horizontal");
        var vMoveDir = Camera.main.transform.forward * Input.GetAxis("Vertical");
        var moveDir = hMoveDir + vMoveDir;

        if (!(moveDir.magnitude > 0)) return;

        // Create the rotation we need according to moveDir
        var lookRotation = Quaternion.LookRotation(moveDir);
        lookRotation.x = 0;
        lookRotation.z = 0;

        // Rotate player over time according to speed until we are in the required rotation
        this.transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);

    }
}

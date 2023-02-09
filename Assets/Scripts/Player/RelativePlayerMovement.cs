using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting.ReorderableList.Internal;
using UnityEngine;
using UnityEngine.Assertions;

public class RelativePlayerMovement : MonoBehaviour
{
    public float speed = 6.0f;
    public float rotationSpeed = 90.0f;

    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * vertical + right * horizontal;

        transform.position += direction * speed * Time.deltaTime;

        if (direction.magnitude > 0)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

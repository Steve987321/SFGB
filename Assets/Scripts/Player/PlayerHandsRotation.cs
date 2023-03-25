using System.Drawing;
using UnityEngine;

class PlayerHandsRotation : MonoBehaviour
{

    private Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        _rb.AddForce(-_rb.angularVelocity * 0.5f);

        //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out var hit))
        //{
        //}
    }
}

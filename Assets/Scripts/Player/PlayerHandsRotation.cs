using System.Drawing;
using UnityEngine;

internal class PlayerHandsRotation : MonoBehaviour
{
    [SerializeField] private Transform _playerHand;

    void Update()
    {
        var mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(mousePos, out var hit)) return;

        Debug.DrawLine(mousePos.origin, hit.point);
        //var direction = hit.point - transform.position;
        //var toRotation = Quaternion.FromToRotation(transform.forward, direction);
        //transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 100 * Time.deltaTime);

        transform.LookAt(hit.point);
        transform.Rotate(0,-90, -90);

    }
}

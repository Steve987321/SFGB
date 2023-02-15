using UnityEngine;

public class RelativePlayerMovement : MonoBehaviour
{
    public float Speed = 6.0f;

    /// <summary>
    /// whether the player is receiving input and is trying to move
    /// </summary>
    public bool IsMoving = false;

    private void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var forward = Camera.main.transform.forward;
        var right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        var direction = forward * vertical + right * horizontal;

        IsMoving = direction.magnitude > 0;

        if (IsMoving)
            transform.position += Speed * Time.deltaTime * direction;
    }


}

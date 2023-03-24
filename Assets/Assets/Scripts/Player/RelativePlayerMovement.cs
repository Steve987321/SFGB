using UnityEngine;

public class RelativePlayerMovement : MonoBehaviour
{
    public float Speed = 6.0f;

    /// <summary>
    /// whether the player is receiving input and is trying to move
    /// </summary>
    public bool IsMoving = false;

    /// <summary>
    /// player is going backwards (does not use input)
    /// </summary>
    public bool IsMovingBackwards = false;

    public Vector3 Direction;

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

        Direction = forward * vertical + right * horizontal;

#if UNITY_EDITOR
        Debug.DrawRay(transform.position, Direction * 10f);
#endif

        IsMoving = Direction.magnitude > 0;

        if (IsMoving)
            transform.position += Speed * Time.deltaTime * Direction;
        else
        {
            IsMovingBackwards = false;
            return;
        }

        // direction relative to input
        var relrot = (
            Quaternion.LookRotation(
                transform.position + Direction - transform.position).eulerAngles
            -
            transform.rotation.eulerAngles
        );

        // threshold
        const float min = 180 - 75f, max = 180 + 75f;
        var tmp = Mathf.Abs(relrot.y);

        IsMovingBackwards = tmp is > min and < max;
    }

#if UNITY_EDITOR

    void OnGUI()
    {
        var res = (
            Quaternion.LookRotation(
            transform.position + Direction - transform.position).eulerAngles
            -
            transform.rotation.eulerAngles
            );
        const float tmp1 = 180f - 75f;
        const float tmp2 = 180f + 75f;

        var res2 = Mathf.Abs(res.y);
        var res3 = res2 > tmp1 && res2 < tmp2 ? "yes" : "no";
        GUI.Label(new Rect(0, 200, 1000, 100), res.ToString());
        GUI.Label(new Rect(0, 220, 1000, 50), res2 +  " " + res3);
    }

#endif


}

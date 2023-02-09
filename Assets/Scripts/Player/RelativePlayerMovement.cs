using UnityEngine;

public class RelativePlayerMovement : MonoBehaviour
{
    public float Speed = 6.0f;

    [SerializeField] private Animator _animatorController;

    private ConfigurableJoint _configurableJoint;
    private Quaternion _initialRot;

    private void Start()
    {
        _configurableJoint = gameObject.GetComponent<ConfigurableJoint>();
        _initialRot = _configurableJoint.transform.localRotation;
    }

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

        if (direction.magnitude > 0)
        {
            _animatorController.SetBool("isMoving", true);
            transform.position += Speed * Time.deltaTime * direction;
        }
        else _animatorController.SetBool("isMoving", false);
    }


}

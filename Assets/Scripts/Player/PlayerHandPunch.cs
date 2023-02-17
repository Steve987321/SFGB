using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHandPunch : MonoBehaviour
{
    [SerializeField] private Transform _root;
    [SerializeField] private PlayerAnimationHandler _animHandler;

    private const float _punchHitThreshold = 10f;
    private const float _punchForce = 50f;

    private Rigidbody _rb;

    private Rigidbody[] _excludeRb;

    void Start()
    {
        _excludeRb = Helper.GetAllRigidBodiesInChildren(_root.gameObject);
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!_animHandler.Punching) return;
        if (_rb.velocity.magnitude > _punchHitThreshold)
        {
            VFXManager.Instance.apply_force(transform.position, _punchForce * 10, 1, _excludeRb);
        }
    }
}

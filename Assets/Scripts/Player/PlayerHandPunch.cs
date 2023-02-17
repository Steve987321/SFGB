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
        _excludeRb = GetAllRigidBodiesInChildren(_root.gameObject);
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!_animHandler.Punching) return;
        if (_rb.velocity.magnitude > _punchHitThreshold)
        {
            VFXManager.Instance.apply_force(transform.position, _punchForce * 10, 1, _excludeRb);
            print("hit");
        }
    }

    public static Rigidbody[] GetAllRigidBodiesInChildren(GameObject root)
    {
        var children = root.GetComponentsInChildren<Transform>();
        var rigidbodies = new Rigidbody[children.Length];
        int count = 0;

        foreach (var child in children)
        {
            var rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rigidbodies[count] = rb;
                count++;
            }
        }

        // Trim array to actual number of rigidbodies
        System.Array.Resize(ref rigidbodies, count);

        return rigidbodies;
    }
}

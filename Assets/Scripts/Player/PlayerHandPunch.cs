using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerHandPunch : NetworkBehaviour
{
    [SerializeField] private Transform _root;
    [SerializeField] private PlayerAnimationHandler _animHandler;

    private const float _punchHitThreshold = 10f;
    private const float _punchForce = 10f;

    private Rigidbody _rb;
     
    private Rigidbody[] _excludeRb = new Rigidbody[19];

    void Start()
    {
        _excludeRb = Helper.GetAllRigidBodiesInChildren(_root.gameObject);
        _rb = GetComponent<Rigidbody>();
    }

    private bool _onceFlag = false;

    void FixedUpdate()
    {
        if (!IsOwner) return;

        if (!_animHandler.Punching)
        {
            _onceFlag = false;
            return;
        }
        if (_rb.velocity.magnitude > _punchHitThreshold && !_onceFlag)
        {
            AudioManager.Instance.Play_Swoosh(transform.position);
            VFXManager.Instance.apply_forceEx(transform.position, _punchForce * 10, 1, _excludeRb);
            var colliders = Physics.OverlapSphere(transform.position, 0.5f);
            foreach (var col in colliders)
            {
                if (_excludeRb.Contains(col.attachedRigidbody)) continue;
                if (col.transform.root.TryGetComponent<Player>(out var player))
                {
                    player.DoDamage(2.5f);

                    AudioManager.Instance.Play_Punch(transform.position);

                    VFXManager.Instance.play_bloodSplatter(transform.position, transform.rotation);
                    //VFXManager.Instance.play_FX(transform.position, transform.rotation, VFXManager.VFX_TYPE.BLOODHIT);
                    _onceFlag = true;
                    break;
                }
            }
        }
    }
}

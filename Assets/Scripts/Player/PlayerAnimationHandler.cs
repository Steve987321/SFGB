using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerAnimationHandler : MonoBehaviour
{
    [Header("essential")]
    [SerializeField] private Transform _PlayerCenter;

    [Header("on aim")]
    [SerializeField] private Vector3 _aimRot;

    private Animator _animController;
    private RelativePlayerMovement _playerMovement;
    private PlayerGunHandler _playerGunHandler;

    private float PunchCooldown = 1.5f;
    private bool random_hand = false;
    
    void Awake()
    {
        _animController = GetComponent<Animator>();
        Debug.Assert(_animController != null, "anim controller is null");

        _playerMovement = _PlayerCenter.GetComponent<RelativePlayerMovement>();
        Debug.Assert(_playerMovement != null, "player movement is null");

        _playerGunHandler = _PlayerCenter.GetComponent<PlayerGunHandler>();
        Debug.Assert(_playerGunHandler != null, "player gun handler is null");

    }

    private float _punchTimer = 0;
    void Update()
    {
        if (_punchTimer < PunchCooldown)
        {
            _punchTimer += Time.deltaTime;
        }

        _animController.SetBool("isMoving", _playerMovement.IsMoving);
        _animController.SetBool("isAiming", _playerGunHandler.HasWeapon);

        if (_playerGunHandler.HasWeapon) return; // don't punch when weapon is held

        // punch
        if (Input.GetKeyDown(KeyCode.Mouse0) && _punchTimer >= PunchCooldown)
        {
            random_hand = !random_hand;
            _animController.Play(random_hand ? "LPunch" : "RPunch");
            _punchTimer = 0;
        }
    }
}

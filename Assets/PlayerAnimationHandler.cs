using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerAnimationHandler : MonoBehaviour
{
    private Animator _animController;

    [SerializeField] private Transform _PlayerCenter;

    private RelativePlayerMovement _playerMovement;
    private PlayerGunHandler _playerGunHandler;

    [Header("on aim")] 
    [SerializeField] private Transform _aimUpperArm;
    [SerializeField] private Vector3 _aimRot;
    private Vector3 _originalAimRot;

    void Awake()
    {
        _animController = GetComponent<Animator>();
        Debug.Assert(_animController != null, "anim controller is null");

        _playerMovement = _PlayerCenter.GetComponent<RelativePlayerMovement>();
        Debug.Assert(_playerMovement != null, "player movement is null");

        _playerGunHandler = _PlayerCenter.GetComponent<PlayerGunHandler>();
        Debug.Assert(_playerGunHandler != null, "player gun handler is null");

        _originalAimRot = _aimUpperArm.localEulerAngles;

    }

    void Update()
    {
        _animController.SetBool("isMoving", _playerMovement.IsMoving);
        _aimUpperArm.localEulerAngles = _playerGunHandler.HasWeapon ? _aimRot : _originalAimRot;
    }

}

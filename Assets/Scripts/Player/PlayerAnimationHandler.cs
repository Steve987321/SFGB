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
    [SerializeField] private Transform _otherUArm;
    [SerializeField] private Transform _otherLArm;
    [Space]
    [SerializeField] private Transform _dominantUArm;
    [SerializeField] private Transform _dominantLArm;

    [Header("on aim")]
    [SerializeField] private Vector3 _aimRot;

    [Header("on charge")]
    [SerializeField] private Vector3 _chargeRotUpper;
    [SerializeField] private Vector3 _chargeRotLower;

    [Header("on punch")] 
    [SerializeField] private Vector3 _punchRotUpper;
    [SerializeField] private Vector3 _punchRotLower;

    [Header("on climb")]
    [SerializeField] private Vector3 _climbRotUpper;
    [SerializeField] private Vector3 _climbRotLower;

    //[SerializeField] private Vector3 _otherUArmPunchRot;
    //[SerializeField] private Vector3 _otherUArmClimbRot;
    //[SerializeField] private Vector3 _otherLArmPunchRot;
    //[SerializeField] private Vector3 _otherLArmClimbRot;
    //[Space]
    //[SerializeField] private Vector3 _dominantUArmPunchRot;
    //[SerializeField] private Vector3 _dominantUArmClimbRot;
    //[SerializeField] private Vector3 _dominantLArmPunchRot;
    //[SerializeField] private Vector3 _dominantLArmClimbRot;

    // how long to press lmb for it to transition to climb state
    private const float HoldWait = 0.7f;
    //private const float ChargeTime = 1.5f;

    private Vector3 _originalOtherUArmRot;
    private Vector3 _originalDominantUArmRot;
    private Vector3 _originalOtherLArmRot;
    private Vector3 _originalDominantLArmRot;

    private Animator _animController;
    private RelativePlayerMovement _playerMovement;
    private PlayerGunHandler _playerGunHandler;


    void Awake()
    {
        _animController = GetComponent<Animator>();
        Debug.Assert(_animController != null, "anim controller is null");

        _playerMovement = _PlayerCenter.GetComponent<RelativePlayerMovement>();
        Debug.Assert(_playerMovement != null, "player movement is null");

        _playerGunHandler = _PlayerCenter.GetComponent<PlayerGunHandler>();
        Debug.Assert(_playerGunHandler != null, "player gun handler is null");

        _originalDominantUArmRot = _dominantUArm.localEulerAngles;
        _originalDominantLArmRot = _dominantLArm.localEulerAngles;
        _originalOtherUArmRot = _otherUArm.localEulerAngles;
        _originalOtherLArmRot = _otherLArm.localEulerAngles;
    }

    private float _timer = 0;
    void Update()
    {
        _animController.SetBool("isMoving", _playerMovement.IsMoving);

        // punching or climbing 
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (_timer < HoldWait)
            {
                _otherUArm.localEulerAngles = _punchRotUpper;
                _dominantUArm.localEulerAngles = Character_Mirror(_punchRotUpper);
                _otherLArm.localEulerAngles = _punchRotLower;
                _dominantLArm.localEulerAngles = Character_Mirror(_punchRotLower);
                _timer += Time.deltaTime;
            }
            else
            {
                _otherUArm.localEulerAngles = _climbRotUpper;
                _dominantUArm.localEulerAngles = Character_Mirror(_climbRotUpper); 
                _otherLArm.localEulerAngles = _climbRotLower;
                _dominantLArm.localEulerAngles =Character_Mirror(_climbRotLower);
            }
        }
        // default 'T-Pose' state
        else
        {
            _dominantUArm.localEulerAngles = _playerGunHandler.HasWeapon ? _aimRot : _originalDominantUArmRot;
            _otherUArm.localEulerAngles = _originalOtherUArmRot;
            _dominantLArm.localEulerAngles = _originalDominantLArmRot;
            _otherLArm.localEulerAngles = _originalOtherLArmRot;

            _timer = 0;
        }
    }


    private Vector3 Character_Mirror(Vector3 vec)
    {
        return new Vector3(vec.x, vec.y * -1, vec.z * -1);
    }
}

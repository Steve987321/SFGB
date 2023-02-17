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

    [SerializeField] private Rigidbody _lHand;
    [SerializeField] private Rigidbody _rHand;

    private Animator _animController;
    private RelativePlayerMovement _playerMovement;
    private PlayerGunHandler _playerGunHandler;

    private const float PunchCooldown = 1.5f;

    /// <summary>
    /// left: true right: false
    /// </summary>
    public bool HittingHand = false;

    /// <summary>
    /// A flag when the player is in a punch animation and should inflict damage
    /// </summary> 
    public bool Punching = false;
    
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
            HittingHand = !HittingHand;
            _animController.Play(HittingHand ? "LPunch" : "RPunch");
            StartCoroutine(_AddForce(HittingHand ? _lHand : _rHand));
            _punchTimer = 0;
        }
    }

    private IEnumerator _AddForce(Rigidbody rb)
    {
        yield return new WaitForSeconds(0.5f);
        Punching = true;
        rb.AddForce(_PlayerCenter.forward * 50, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);
        Punching = false;
    }
}

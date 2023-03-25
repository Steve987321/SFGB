using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAiming : MonoBehaviour
{
    [SerializeField] private Transform _PlayerCenter;
    [SerializeField] private Transform _animatorAimObj;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _rbAimObj;

    [SerializeField] private Vector3 _aimOffset;

    private PlayerGunHandler _playerGunHandler;

    void Awake()
    {
        _playerGunHandler = _PlayerCenter.GetComponent<PlayerGunHandler>();
        Debug.Assert(_playerGunHandler != null, "player gun handler is null");
    }

    // TODO: remove aim animation ?
    // after animation positions get applied
    void LateUpdate()
    {
        if (_playerGunHandler.HasWeapon)
        {
            //var pos = Camera.main.ScreenToWorldPoint(new Vectoasr3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 100f, 1 << _layerMask.value))
            {
                Debug.DrawLine(hit.point, _rbAimObj.position);
                var target = Quaternion.LookRotation(hit.point - _rbAimObj.position) * Quaternion.Euler(_playerGunHandler.EquippedWeapon.WeaponType == Weapon.WEAPON.RPG ? new Vector3(-85.4f, -138.7f, 0f) : _aimOffset);
                _animatorAimObj.rotation = target;
            }
        }
    }
}

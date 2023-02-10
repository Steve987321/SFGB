using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WEAPON
    {
        NONE,
        AK,
        SHOTGUN,
        M4,
        PISTOL,
        RPG,
        SMG
    }

    private WEAPON _weaponType;

    private bool _canShoot = false;
    private bool _canReload = false;

    [SerializeField] private int _maxAmmo;
    
    public float Recoil;
    public float FireDelay;
    public float ReloadDelay;
    public float Damage;
    public int Ammo;

    void Start()
    {
        _maxAmmo = Ammo;
    }

    public void Shoot()
    {

        if (Ammo == 0)
        {
            Reload();
            return;
        }

        if (_canShoot)
            StartCoroutine(_Shoot());
    }

    private IEnumerator _Shoot()
    {
        _canShoot = false;
        if (Physics.Raycast(transform.position, Vector3.forward, out var hit))
        {
            Debug.Log("hit: " + hit.collider.name);
        }
        yield return new WaitForSeconds(FireDelay);
        _canShoot = true;
    }

    public void Reload()
    {
        if (Ammo < _maxAmmo && _canReload)
            StartCoroutine(_Reload());
    }

    private IEnumerator _Reload()
    {
        _canReload = false;

        // reload animation

        yield return new WaitForSeconds(ReloadDelay);

        Ammo = _maxAmmo;

        _canReload = true;
    }
    
    
}

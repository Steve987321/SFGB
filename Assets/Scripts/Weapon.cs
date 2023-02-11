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

    [System.Serializable]
    public class gunFX
    {
        public VFXManager.VFX_TYPE type;
        public Transform transform;
    }

    private bool _canShoot = false;
    private bool _canReload = false;

    /*[SerializeField]*/ private int _maxAmmo;

    [Tooltip("The FX that plays on shooting")]
    public gunFX[] _gunFXs;

    [Tooltip("The FX that plays on reloading")]
    public gunFX[] _gunReloadFxs;

    public WEAPON WeaponType = WEAPON.NONE;
    public float Recoil;
    public float FireDelay;
    public float ReloadDelay;
    public float Damage;
    public int Ammo;

    void Start()
    {

        if (_gunFXs.Length == 0)
        {
            Debug.LogError("make sure you have atleast one FX for selected on the weapon component");
        }


        _canShoot = true;
        _maxAmmo = Ammo;
    }

    public void Shoot()
    {
        Debug.Log("shooting");
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

        foreach (var t in _gunFXs)
        {
            VFXManager.Instance.play_FX(t.transform, t.type);
        }

        var colliders = Physics.OverlapSphere(transform.position, 2);
        foreach (var col in colliders)
        {
            var rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(Recoil * 100f, transform.position, 2);
            }
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
        if (_gunReloadFxs.Length != 0 )
            foreach (var t in _gunReloadFxs)
            {
                VFXManager.Instance.play_FX(t.transform, t.type);
            }

        yield return new WaitForSeconds(ReloadDelay);

        Ammo = _maxAmmo;

        _canReload = true;
    }
    
    
}

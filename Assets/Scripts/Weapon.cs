using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public struct gunFX
    {
        public VFXManager.VFX_TYPE Type;
        public Transform Transform;
    }

    private bool _canShoot = false;

    [SerializeField] private Transform _endOfBarrel;

    [Tooltip("The FX that plays on shooting")]
    public gunFX[] GunFxs;

    public WEAPON WeaponType = WEAPON.NONE;

    [Space]

    [Header("set this when RPG is the weapon type")]
    [SerializeField] private GameObject RocketObj;

    public float RocketSpeed = 10;

    [Header("Weapon Stats")]
    public float Recoil;
    public float FireDelay;
    public float Damage;
    public int Ammo;

    [Tooltip("the force of impact")]
    public float BulletForce;

    private List<Transform> _rbInRoot = new List<Transform>();

    void Start()
    {
        if (GunFxs.Length == 0)
        {
            Debug.LogError("make sure atleast one FX is selected on the weapon component, from" + this.name);
        }
        if (transform.root.gameObject.name.Contains("player", StringComparison.OrdinalIgnoreCase))
            _rbInRoot.AddRange(Helper.GetAllTransformsInChildren(transform.root.gameObject).ToList());
        _canShoot = true;
    }

    public void Shoot()
    {
        if (_canShoot)
            StartCoroutine(_Shoot());
    }

    private IEnumerator _Shoot()
    {
        _canShoot = false;
        Debug.DrawRay(_endOfBarrel.position, _endOfBarrel.forward, Color.red, 3);

        if (WeaponType == WEAPON.RPG)
        {
            var shotRocket = Instantiate(RocketObj);
            shotRocket.transform.SetPositionAndRotation(RocketObj.transform.position, RocketObj.transform.rotation);
            Destroy(RocketObj);
            var rocket = shotRocket.AddComponent<Rocket>();
            var rb = shotRocket.AddComponent<Rigidbody>();
            rb.mass = 0.5f;
            rb.useGravity = false;
            rb.AddForce(25 * -shotRocket.transform.forward, ForceMode.Impulse);
            rocket.PlayerTransform = transform;
            rocket.ExcludeObj.AddRange(_rbInRoot);

            // destroy the rocket after some seconds in case nothing has been hit 
            //Destroy(rocket.gameObject, 7);
        }
        else
        {
            if (Physics.Raycast(_endOfBarrel.position, _endOfBarrel.forward, out var hit))
            {
                print("hit: " + hit.collider.name);
                VFXManager.Instance.apply_force(hit.point, BulletForce * 100, 3);
            }
        }
        
        foreach (var t in GunFxs)
        {
            VFXManager.Instance.play_FX(t.Transform, t.Type);
        }

        // apply recoil
        VFXManager.Instance.apply_force(transform.position, Recoil * 100f, 2);

        yield return new WaitForSeconds(FireDelay);
        if (WeaponType != WEAPON.RPG) _canShoot = true;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerGunHandler : MonoBehaviour
{
    [SerializeField] private Transform _gunHand;

    private GameObject _weapon;
    private float _gunRBMass;
    private Rigidbody _gunHandRb;

    // whether the player has a weapon equipped
    public bool HasWeapon = false;

    void Start()
    {
        _gunHandRb = _gunHand.parent.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!HasWeapon)
        {
            pickUpGunHandler();
        }
        else
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                _weapon.GetComponent<Weapon>().Shoot();
            }
            else if (Input.GetKeyDown(KeyCode.G)) // drop
            {
                Debug.Log("throwing away a gun");
                var f = Instantiate(_weapon);

                f.transform.localScale = _weapon.GetComponent<Weapon>().gameObject.transform.lossyScale;

                f.transform.SetPositionAndRotation(_gunHand.transform.position, _gunHand.transform.rotation);

                f.AddComponent<Rigidbody>().mass = _gunRBMass;

                f.tag = "Gun";
                Destroy(_weapon);
                _gunHandRb.mass = 1.0f;
                HasWeapon = false;
            }
            
        }
    }

    private void pickUpGunHandler()
    {
        var transforms = GameObject.FindGameObjectsWithTag("Gun").Select(gun => gun.transform);
        var closestGun = Helper.GetClosest(transform, transforms.ToArray());
        if (closestGun == null) return;
        if (!Helper.IsInReach(closestGun.position, transform.position, 3.0f)) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            _gunRBMass = closestGun.GetComponent<Rigidbody>().mass;

            var go = Instantiate(closestGun);
            Destroy(go.GetComponent<Rigidbody>());
            go.SetPositionAndRotation(_gunHand.position, _gunHand.rotation);
            if (go.GetComponent<Weapon>().WeaponType == Weapon.WEAPON.RPG)
                go.rotation *= Quaternion.Euler(-4.9f, -20.8f, 0.0f);
            go.parent = _gunHand;
            go.tag = "Untagged";
            Destroy(closestGun.gameObject);
            _weapon = go.gameObject;
            _gunHandRb.mass = 0.2f;
            HasWeapon = true;
        }
    }

}

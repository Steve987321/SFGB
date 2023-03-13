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

    private struct Arm
    {
        public Rigidbody rb;
        public float ogDrag;
        public float ogAngularDrag;
    }

    private GameObject _weapon;
    private float _gunRBMass;

    private Arm _gunHandRb, _gunUArmRb, _gunLArmRb;

    private float ogVal;

    // whether the player has a weapon equipped
    public bool HasWeapon = false;
    public Weapon EquippedWeapon;

    void Start()
    {
        _gunHandRb.rb = _gunHand.parent.GetComponent<Rigidbody>();
        _gunLArmRb.rb = _gunHand.parent.parent.GetComponent<Rigidbody>();
        _gunUArmRb.rb = _gunHand.parent.parent.parent.GetComponent<Rigidbody>();

        _gunHandRb.ogAngularDrag = _gunHandRb.rb.angularDrag;
        _gunHandRb.ogDrag = _gunHandRb.rb.drag;
        _gunLArmRb.ogAngularDrag = _gunLArmRb.rb.angularDrag;
        _gunLArmRb.ogDrag = _gunLArmRb.rb.drag;
        _gunUArmRb.ogAngularDrag = _gunUArmRb.rb.angularDrag;
        _gunUArmRb.ogDrag = _gunUArmRb.rb.drag;

    }

    void Update()
    {
        if (!HasWeapon)
        {
            PickUpGunHandler();
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

                foreach (Transform t in _weapon.transform)
                {
                    if (!t.CompareTag("GunUtil")) continue;

                    if (t.TryGetComponent<Light>(out var l))
                        l.enabled = false;
                }

                var f = Instantiate(_weapon);

                f.transform.localScale = _weapon.GetComponent<Weapon>().gameObject.transform.lossyScale;

                f.transform.SetPositionAndRotation(_gunHand.transform.position, _gunHand.transform.rotation);

                f.AddComponent<Rigidbody>().mass = _gunRBMass;

                f.tag = "Gun";
                Destroy(_weapon);

                _gunHandRb.rb.drag = _gunHandRb.ogDrag;
                _gunHandRb.rb.angularDrag = _gunHandRb.ogAngularDrag;

                _gunLArmRb.rb.drag = _gunLArmRb.ogDrag;
                _gunLArmRb.rb.angularDrag = _gunLArmRb.ogAngularDrag; 

                _gunUArmRb.rb.drag = _gunUArmRb.ogDrag;
                _gunUArmRb.rb.angularDrag = _gunUArmRb.ogAngularDrag;

                _gunHandRb.rb.mass = 1.0f;

                HasWeapon = false;
                EquippedWeapon = null;
            }
            
        }
    }

    private void PickUpGunHandler()
    {
        var transforms = GameObject.FindGameObjectsWithTag("Gun").Where(gun => gun.transform.parent == null).Select(gun => gun.transform);

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

            foreach (Transform t in go.transform)
            {
                if (!t.CompareTag("GunUtil")) continue;

                if (t.TryGetComponent<Light>(out var l))
                    l.enabled = true;
            }

            // helps with aiming
            _gunHandRb.rb.drag = 25;
            _gunHandRb.rb.angularDrag = 25;

            _gunLArmRb.rb.drag = 25;
            _gunLArmRb.rb.angularDrag = 25;
            
            _gunUArmRb.rb.drag = 25;
            _gunUArmRb.rb.angularDrag = 25;

            go.parent = _gunHand;
            go.tag = "Untagged";
            Destroy(closestGun.gameObject);
            _weapon = go.gameObject;
            _gunHandRb.rb.mass = 0.2f;

            HasWeapon = true;
            EquippedWeapon = go.GetComponent<Weapon>();
        }
    }

}

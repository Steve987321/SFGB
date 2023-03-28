using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

public class PlayerGunHandler : NetworkBehaviour
{
    [SerializeField] private Transform _gunHand;

    private List<Collider> _lplayerCollissions;

    private struct Arm
    {
        public Rigidbody rb;
        public float ogDrag;
        public float ogAngularDrag;
    }

    private GameObject _weapon;

    private Arm _gunHandRb, _gunUArmRb, _gunLArmRb;

    private float _gunRBMass;

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

        if (IsOwner)
        {
            _lplayerCollissions = new List<Collider>();
            foreach (Transform t in transform)
            {
                if (t.TryGetComponent<Collider>(out var col))
                    _lplayerCollissions.Add(col);
            }
        }
    }

    void Update()
    {
        if (!IsOwner)
            return;

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
            else if (Input.GetKeyDown(KeyCode.G))
            {
                DropWeapon();
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

            var offset = closestGun.GetComponent<Weapon>().WeaponType == Weapon.WEAPON.RPG
                ? new Vector3(-4.9f, -20.8f, 0.0f)
                : Vector3.zero;


            foreach (var playerCol in _lplayerCollissions)
            {
                print(closestGun.name + " Ignores " + playerCol.name);
                Physics.IgnoreCollision(closestGun.GetComponent<Collider>(), playerCol, true); // should ignore

                foreach (Transform gunT in closestGun.transform)
                    if (gunT.TryGetComponent<Collider>(out var gunCol))
                    {
                        print(gunCol.name + " Ignores " + playerCol.name);
                        Physics.IgnoreCollision(gunCol, playerCol, true); // should ignore
                    }
            }
               

            if (closestGun.TryGetComponent<TransformFollower>(out var tfollower))
                tfollower.SetTarget(_gunHand, offset);
            else
                closestGun.AddComponent<TransformFollower>().SetTarget(_gunHand, offset);

            //go.SetPositionAndRotation(_gunHand.position, _gunHand.rotation);

            //if (go.GetComponent<Weapon>().WeaponType == Weapon.WEAPON.RPG)
            //    go.rotation *= Quaternion.Euler(-4.9f, -20.8f, 0.0f);

            foreach (Transform t in closestGun.transform)
            {
                if (!t.CompareTag("GunUtil")) continue;

                if (t.TryGetComponent<Light>(out var l))
                    l.enabled = true;
            }

            // helps with aiming
            _gunHandRb.rb.drag = 15;
            _gunHandRb.rb.angularDrag = 15;

            _gunLArmRb.rb.drag = 15;
            _gunLArmRb.rb.angularDrag = 15;
            
            _gunUArmRb.rb.drag = 15;
            _gunUArmRb.rb.angularDrag = 15;

            //go.parent = _gunHand;
            closestGun.tag = "Untagged";
            _weapon = closestGun.gameObject;
            _gunHandRb.rb.mass = 0.2f;

            HasWeapon = true;
            EquippedWeapon = closestGun.GetComponent<Weapon>();
            closestGun.GetComponent<Weapon>().IsEquippedByPlayer = true;
        }
    }

    private void DropWeapon()
    {
        Destroy(_weapon.GetComponent<TransformFollower>());

        foreach (var playerCol in _lplayerCollissions)
        {
            Physics.IgnoreCollision(_weapon.GetComponent<Collider>(), playerCol, true); // should ignore
            foreach (Transform gunT in _weapon.transform)
                if (gunT.TryGetComponent<Collider>(out var gunCol))
                    Physics.IgnoreCollision(gunCol, playerCol, false); // should not ignore
        }
      

        foreach (Transform t in _weapon.transform)
        {
            if (!t.CompareTag("GunUtil")) continue;

            if (t.TryGetComponent<Light>(out var l))
                l.enabled = false;
        }

        //var f = Instantiate(_weapon);

        //f.transform.localScale = _weapon.GetComponent<Weapon>().gameObject.transform.lossyScale;

        //f.transform.SetPositionAndRotation(_gunHand.transform.position, _gunHand.transform.rotation);

        //f.AddComponent<Rigidbody>().mass = _gunRBMass;

        _weapon.tag = "Gun";

        _gunHandRb.rb.drag = _gunHandRb.ogDrag;
        _gunHandRb.rb.angularDrag = _gunHandRb.ogAngularDrag;

        _gunLArmRb.rb.drag = _gunLArmRb.ogDrag;
        _gunLArmRb.rb.angularDrag = _gunLArmRb.ogAngularDrag;

        _gunUArmRb.rb.drag = _gunUArmRb.ogDrag;
        _gunUArmRb.rb.angularDrag = _gunUArmRb.ogAngularDrag;

        _gunHandRb.rb.mass = 1.0f;

        HasWeapon = false;
        EquippedWeapon = null;

        foreach (var playerCol in _lplayerCollissions)
            Physics.IgnoreCollision(_weapon.GetComponent<Collider>(), playerCol, false);

        _weapon.GetComponent<Weapon>().IsEquippedByPlayer = false;
    }

}

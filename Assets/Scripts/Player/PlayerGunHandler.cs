using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Weapon;

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

            Helper.IterateThroughTransform(
                transform, 
                (t) => {
                    if (t.TryGetComponent<Collider>(out var col))
                        _lplayerCollissions.Add(col);
                }, 
                true
            );
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner)
            return;

        if (!HasWeapon) 
            PickUpGunHandler();
        else {
            if (Input.GetKey(KeyCode.Mouse0)) 
                _weapon.GetComponent<Weapon>().Shoot();
            else if (Input.GetKey(KeyCode.G))
                DropWeapon();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SetWeaponTransformTargetServerRpc(NetworkObjectReference gun)
    {
        print("SERVER");
        gun.TryGet(out var wepon, NetworkManager.Singleton);
        pickup(wepon.transform);
    } 

    [ClientRpc]
    void SetWeaponTransformTargetClientRpc(NetworkObjectReference gun)
    {
        print("CLIENT");

        gun.TryGet(out var closestGun);
        var offset = closestGun.GetComponent<Weapon>().WeaponType == Weapon.WEAPON.RPG
            ? new Vector3(-4.9f, -20.8f, 0.0f)
            : Vector3.zero;

        var gunRB = closestGun.GetComponent<Rigidbody>();
        var gunCol = closestGun.GetComponent<Collider>();

        _gunRBMass = gunRB.mass;

        gunCol.enabled = false;
        Helper.IterateThroughTransform(closestGun.transform,
            (t) =>
            {
                if (t.TryGetComponent<Collider>(out var col)) col.enabled = false;
            });

        gunRB.mass = 1;
        gunRB.useGravity = false;
        gunRB.isKinematic = false;

        // add drag for smoother aim
        _gunHandRb.rb.drag = 25;
        _gunHandRb.rb.angularDrag = 25;

        _gunLArmRb.rb.drag = 25;
        _gunLArmRb.rb.angularDrag = 25;

        _gunUArmRb.rb.drag = 25;
        _gunUArmRb.rb.angularDrag = 25;

        //go.parent = _gunHand;
        closestGun.tag = "Untagged";
        _weapon = closestGun.gameObject;
        _gunHandRb.rb.mass = 0.2f;

        EquippedWeapon = closestGun.GetComponent<Weapon>();
        EquippedWeapon.IsEquippedByPlayer = true;

        if (closestGun.TryGetComponent<TransformFollower>(out var tfollower))
            tfollower.SetTarget(_gunHand, offset);
    }

    void pickup(Transform closestGun)
    {
        var offset = closestGun.GetComponent<Weapon>().WeaponType == Weapon.WEAPON.RPG
            ? new Vector3(-4.9f, -20.8f, 0.0f)
            : Vector3.zero;

        var gunRB = closestGun.GetComponent<Rigidbody>();
        var gunCol = closestGun.GetComponent<Collider>();

        _gunRBMass = gunRB.mass;

        gunCol.enabled = false;
        Helper.IterateThroughTransform(closestGun.transform,
            (t) =>
            {
                if (t.TryGetComponent<Collider>(out var col)) col.enabled = false;
            });

        gunRB.mass = 1;
        gunRB.useGravity = false;
        gunRB.isKinematic = false;

        // add drag for smoother aim
        _gunHandRb.rb.drag = 25;
        _gunHandRb.rb.angularDrag = 25;

        _gunLArmRb.rb.drag = 25;
        _gunLArmRb.rb.angularDrag = 25;

        _gunUArmRb.rb.drag = 25;
        _gunUArmRb.rb.angularDrag = 25;

        //go.parent = _gunHand;
        closestGun.tag = "Untagged";
        _weapon = closestGun.gameObject;
        _gunHandRb.rb.mass = 0.2f;

        EquippedWeapon = closestGun.GetComponent<Weapon>();
        EquippedWeapon.IsEquippedByPlayer = true;

        if (closestGun.TryGetComponent<TransformFollower>(out var tfollower))
            tfollower.SetTarget(_gunHand, offset);
    }

    void SetWeaponTransformTarget(NetworkObjectReference gun)
    {
        print("try picking up gun");

        SetWeaponTransformTargetServerRpc(gun);
    }

    void PickUpGunHandler()
    {
        var transforms = GameObject.FindGameObjectsWithTag("Gun").Where(gun => gun.transform.parent == null).Select(gun => gun.transform);

        var closestGun = Helper.GetClosest(transform, transforms.ToArray());
        if (closestGun == null) return;
        if (!Helper.IsInReach(closestGun.position, transform.position, 3.0f)) return;

        if (Input.GetKey(KeyCode.E))
        {
            //SetWeaponTransformTarget(closestGun.GetComponent<NetworkObject>());

            HasWeapon = true;
            SetWeaponTransformTargetServerRpc(closestGun.GetComponent<NetworkObject>());
            pickup(closestGun);
            //go.SetPositionAndRotation(_gunHand.position, _gunHand.rotation);

            //if (go.GetComponent<Weapon>().WeaponType == Weapon.WEAPON.RPG)
            //    go.rotation *= Quaternion.Euler(-4.9f, -20.8f, 0.0f);

            foreach (Transform t in closestGun.transform)
            {
                if (!t.CompareTag("GunUtil")) continue;

                if (t.TryGetComponent<Light>(out var l))
                    l.enabled = true;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropWeaponServerRpc()
    {
         DropWeaponClientRpc();   
    }

    [ClientRpc]
    private void DropWeaponClientRpc()
    {
        _weapon.GetComponent<TransformFollower>().SetTarget(null);

        //foreach (var playerCol in _lplayerCollissions)
        //{
        //    Physics.IgnoreCollision(_weapon.GetComponent<Collider>(), playerCol, true); // should ignore
        //    foreach (Transform gunT in _weapon.transform)
        //        if (gunT.TryGetComponent<Collider>(out var gunCol))
        //            Physics.IgnoreCollision(gunCol, playerCol, false); // should not ignore
        //}

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

        var gunRB = _weapon.GetComponent<Rigidbody>();
        var gunCol = _weapon.GetComponent<Collider>();

        Helper.IterateThroughTransform(_weapon.transform,
            (t) =>
            {
                if (t.TryGetComponent<Collider>(out var col)) col.enabled = false;
            });

        gunCol.enabled = true;
        gunRB.mass = _gunRBMass;
        gunRB.useGravity = true;
        gunRB.isKinematic = false;

        _gunHandRb.rb.drag = _gunHandRb.ogDrag;
        _gunHandRb.rb.angularDrag = _gunHandRb.ogAngularDrag;

        _gunLArmRb.rb.drag = _gunLArmRb.ogDrag;
        _gunLArmRb.rb.angularDrag = _gunLArmRb.ogAngularDrag;

        _gunUArmRb.rb.drag = _gunUArmRb.ogDrag;
        _gunUArmRb.rb.angularDrag = _gunUArmRb.ogAngularDrag;

        _gunHandRb.rb.mass = 1.0f;

        HasWeapon = false;
        EquippedWeapon = null;

        //foreach (var playerCol in _lplayerCollissions)
        //    Physics.IgnoreCollision(_weapon.GetComponent<Collider>(), playerCol, false);

        _weapon.GetComponent<Weapon>().IsEquippedByPlayer = false;
    }

    void DropWeapon()
    {
        DropWeaponServerRpc();  
    }
}

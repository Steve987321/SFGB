using System.Linq;
using System.Security.Cryptography;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHandGrips : NetworkBehaviour
{
    [SerializeField] private PlayerGunHandler _gunHandler;
    public bool IsDominantHand = false;

    [HideInInspector] public GameObject HeldObject;

    // whether gripping key is pressed
    private bool _grippingBtnDown = false;

    // wheter player is holding an object
    private bool _isHolding = false;

    private Rigidbody _rb;

    private ulong _heldObjectId;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    //void FixedUpdate()
    //{
    //    if (!IsOwner)
    //        return;

    //    _grippingBtnDown = Input.GetKey(KeyCode.Mouse1);

    //    if (!_grippingBtnDown && _isHolding)
    //    {
    //        print("released " + HeldObject.name);
    //        if (HeldObject.GetComponent<FixedJoint>() != null)
    //            Destroy(HeldObject.GetComponent<FixedJoint>());
    //        //Release(HeldObject);
    //        _isHolding = false;
    //    }

    //    if (IsDominantHand && _gunHandler.HasWeapon) return;
    //    if (_isHolding) return;
    //    if (!_grippingBtnDown) return;

    //    // look for object to grab with prop tag
    //    var list = GameObject.FindGameObjectsWithTag("Prop").Select(item => item.transform);
    //    var transforms = list as Transform[] ?? list.ToArray();
    //    if (transforms.Length <= 0) return;

    //    var item = Helper.GetClosest(transform, transforms);
    //    if (item == null) return;

    //    var collider = item.GetComponent<Collider>();
    //    //Debug.Log(Vector3.Distance(transform.position, collider.ClosestPoint(transform.position)));
    //    //Debug.DrawLine(collider.ClosestPoint(transform.position), transform.position);
    //    if (!Helper.IsInReach(collider.ClosestPoint(transform.position), transform.position, 0.2f)) return;

    //    if (item.GetComponent<FixedJoint>() == null)
    //    {
    //        var j = item.AddComponent<FixedJoint>();
    //        j.connectedBody = _rb;
    //    }

    //    HeldObject = item.gameObject;
    //    print("grabbing: " + HeldObject.name);
    //    _isHolding = true;

    //    // Debug.Log(item.name);
    //}

    void FixedUpdate()
    {
        if (!IsOwner)
            return;

        _grippingBtnDown = Input.GetKey(KeyCode.Mouse1);

        if (!_grippingBtnDown && _isHolding)
        {
            print("released " + HeldObject.name);
            //releaseObject(_heldObjectId);
            ReleaseObjectServerRpc(_heldObjectId);
            _isHolding = false;
        }

        if (IsDominantHand && _gunHandler.HasWeapon) return;
        if (_isHolding) return;
        if (!_grippingBtnDown) return;

        // look for object to grab with prop tag
        var list = GameObject.FindGameObjectsWithTag("Prop").Select(item => item.transform);
        var transforms = list as Transform[] ?? list.ToArray();
        if (transforms.Length <= 0) return;

        var item = Helper.GetClosest(transform, transforms);
        if (item == null) return;

        var collider = item.GetComponent<Collider>();
        if (!Helper.IsInReach(collider.ClosestPoint(transform.position), transform.position, 0.2f)) return;

        _heldObjectId = item.GetComponent<NetworkObject>().NetworkObjectId;

        //grabObject(_heldObjectId);
        GrabObjectServerRpc(_heldObjectId);

    }

    void releaseObject(ulong nId)
    {
        var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[nId];
        if (obj.GetComponent<FixedJoint>() != null)
            Destroy(obj.GetComponent<FixedJoint>());
    }

    void grabObject(ulong nId)
    {
        var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[nId];
        if (obj.GetComponent<FixedJoint>() == null)
        {
            var j = obj.AddComponent<FixedJoint>();
            j.connectedBody = _rb;
        }

        HeldObject = obj.gameObject;
        print("grabbing: " + HeldObject.name);
        _isHolding = true;
    }

    [ServerRpc]
    private void GrabObjectServerRpc(ulong nId)
    {
        //GrabObjectClientRpc(nId);
        grabObject(nId);
    }

    [ServerRpc]
    private void ReleaseObjectServerRpc(ulong nId)
    {
        //ReleaseObjectClientRpc(nId);
        releaseObject(nId);
    }

    [ClientRpc]
    void GrabObjectClientRpc(ulong nId)
    {
        var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[nId];
        if (obj.GetComponent<FixedJoint>() == null)
        {
            var j = obj.AddComponent<FixedJoint>();
            j.connectedBody = _rb;
        }

        HeldObject = obj.gameObject;
        print("grabbing: " + HeldObject.name);
        _isHolding = true;
    }

    [ClientRpc]
    void ReleaseObjectClientRpc(ulong nId)
    {
        var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[nId];
        if (obj.GetComponent<FixedJoint>() != null)
            Destroy(obj.GetComponent<FixedJoint>());
    }

}

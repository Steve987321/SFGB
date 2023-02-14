using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHands : MonoBehaviour
{
    [HideInInspector] public GameObject HeldObject;
    
    private bool _mdown = false;
    private bool _isHolding = false;

    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _mdown = Input.GetKey(KeyCode.Mouse1);
        
        if (_isHolding) return;
        if (!_mdown) return;

        var list = GameObject.FindGameObjectsWithTag("Prop").Select(item => item.transform);
        var transforms = list as Transform[] ?? list.ToArray();
        if (transforms.Length <= 0) return;

        var item = Helper.GetClosest(transform, transforms);
        if (item == null) return;

        var collider = item.GetComponent<Collider>(); 
        //Debug.Log(Vector3.Distance(transform.position, collider.ClosestPoint(transform.position)));
        //Debug.DrawLine(collider.ClosestPoint(transform.position), transform.position);
        if (!Helper.IsInReach(collider.ClosestPoint(transform.position), transform.position, 0.2f)) return;

        if (item.GetComponent<FixedJoint>() == null)
        {
            var j = item.AddComponent<FixedJoint>();
            j.connectedBody = _rb;
        }

        HeldObject = item.gameObject;
        Debug.Log("grabbing: " + HeldObject.name);
        _isHolding = true;

        // Debug.Log(item.name);
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (_mdown && !_isHolding)
        //{
        //    if (!collision.gameObject.CompareTag("Prop")) return;

        //    if (collision.gameObject.GetComponent<FixedJoint>() == null)
        //    {
        //        var j = collision.gameObject.AddComponent<FixedJoint>();
        //        j.connectedBody = _rb;
        //    }
        //    HeldObject = collision.gameObject;
        //    _isHolding = true;
        //}
    }


    void FixedUpdate()
    {
        if (!_mdown && _isHolding)
        {
            Debug.Log("released " + HeldObject.name);
            if (HeldObject.GetComponent<FixedJoint>() != null)
                Destroy(HeldObject.GetComponent<FixedJoint>());
            _isHolding = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerGunHandler : MonoBehaviour
{
    [SerializeField] private Animator _animatorController;
    [SerializeField] private Transform _gunHand;

    private Weapon _weapon = null;

    void Start()
    {
        
    }

    void Update()
    {
        if (_weapon != null)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                _weapon.Shoot();
            }
        }
        else
        {
            // no weapon equipped so we see if we can pick up a weapon
            pickUpGunHandler();
        }

    
    }


    private void pickUpGunHandler()
    {
        var transforms = GameObject.FindGameObjectsWithTag("Gun").Select(gun => gun.transform);
        var closestGun = Helper.GetClosest(transform, transforms.ToArray());
        if (closestGun == null) return;
        if (!Helper.IsInReach(closestGun.position, transform.position, 2.5f)) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Instantiate(closestGun, _gunHand).parent = _gunHand;
            Destroy(closestGun.gameObject);
            _weapon = closestGun.GetComponent<Weapon>();
        }
    }

}

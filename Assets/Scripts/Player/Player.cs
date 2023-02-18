using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _health = 100;

    public void DoDamage(float val)
    {
        _health -= val;
        if (_health < 0)
        {
            _health = 0;
            Die();
        }
    }

    void Die()
    {
        var rbs = Helper.GetAllRigidBodiesInChildren(gameObject);
        foreach (var rb in rbs)
        {
            if (rb.transform.TryGetComponent<ConfigurableJoint>(out var joint))
            {
                bool center = string.Compare(joint.name, "hip(CENTER)", StringComparison.OrdinalIgnoreCase) == 0;
                var componentAngularXDrive = joint.angularXDrive;
                componentAngularXDrive.positionSpring = center ? 0 : 150;
                joint.angularXDrive = componentAngularXDrive;

                var componentAngularYzDrive = joint.angularYZDrive;
                componentAngularYzDrive.positionSpring = center ? 0 : 150;
                joint.angularYZDrive = componentAngularYzDrive;
            }
        }
    }
}

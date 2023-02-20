using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public float Health = 100f;

    public void DoDamage(float val)
    {
        Health -= val;
        if (Health < 0)
        {
            Health = 0;
            Die();
        }

        // cam shake on damage to any player

        CameraManager.Instance.DoShake(0.3f, 3f, 0.7f);

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

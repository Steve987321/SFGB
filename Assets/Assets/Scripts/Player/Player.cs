using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Health = 100f;

    public void DoDamage(float val)
    {
        Health -= val;

        float t = Mathf.Clamp01(1f - Health / 100f);

        if (t < 0.5f) // make effects notacible after half health
            t /= 3f;

        // check if localplayer when adding multiplayer
        AudioManager.Instance.SetDistortion(t / 2f); // max should be .5f

        AudioManager.Instance.SetParamEQ(
            Mathf.Lerp(8000f, 7600f, t),
            Mathf.Lerp(1f, 3f, t),
            Mathf.Lerp(1f, 0.1f, t)
            );

        AudioManager.Instance.CenterFreqLimit = Mathf.Lerp(8000f, 7600f, t);
        AudioManager.Instance.OctaveRangeLimit = Mathf.Lerp(1f, 3f, t);
        AudioManager.Instance.FreqGainLimit = Mathf.Lerp(1f, 0.1f, t);

        VFXManager.Instance.SetFilmGrain(t);

        if (Health < 0)
        {
            Health = 0;
            Die();
        }

        // cam shake on damage to any player
        CameraManager.Instance.DoShake(0.3f, 3f, 0.7f);
    }

    /*
     * turn player in ragdoll 
     */
    private void Die()
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

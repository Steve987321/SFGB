using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public NetworkVariable<float> Health = new(100f);

    public NetworkVariable<bool> IsReady = new(false);

    public float Cooldown = 0.1f;

    void Start()
    {
        if (!IsOwner)
            return;

        var ignoreLayer = LayerMask.NameToLayer("IgnoreAim");

        transform.gameObject.layer = ignoreLayer;
        Helper.SetChildLayers(ignoreLayer, transform, true);
    }

    void Update()
    {
        if (Cooldown > 0)
        {
            Cooldown -= Time.deltaTime;
        }
    }

    /// <summary>
    /// applies damage to this player
    /// </summary>
    /// <param name="val"> Amount of damage. </param>
    public void DoDamage(float val)
    {
        if (Cooldown > 0) return;

        DoDamageServerRpc(val);

        float t = Mathf.Clamp01(1f - Health.Value / 100f);

        if (t < 0.5f) // make effects notacible after half health
            t /= 3f;

        if (IsOwner)
        {
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
        }

        // cam shake on damage to any player
        CameraManager.Instance.DoShake(0.3f, 3f, 0.7f);
        Cooldown = 0.1f;
    }

    [ServerRpc(RequireOwnership = false)]
    void DoDamageServerRpc(float val)
    {
        Health.Value -= val;
        
        if (Health.Value <= 0)
        {
            Health.Value = 0;
            Die();
        }
    }

    public void SetReady(bool readystate)
    {
        SetReadyServerRpc(readystate);
    }

    [ServerRpc(RequireOwnership = false)]
    void SetReadyServerRpc(bool readystate)
    {
        IsReady.Value = readystate;
    }

    /*
     * turn player in ragdoll 
     */
    private void Die()
    {
        DieServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void DieServerRpc(){
        DieClientRpc();
    }

    [ClientRpc]
    void DieClientRpc()
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

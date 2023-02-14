using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public enum VFX_TYPE
    {
        SMOKE,
        MUZZLEFLASH
    }

    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private ParticleSystem _sparks;
    [SerializeField] private ParticleSystem _smokeGun;
    [SerializeField] private ParticleSystem _smokeBig;

    public static VFXManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void play_FX(Transform at, VFX_TYPE type)
    {
        switch (type)
        {
            case VFX_TYPE.SMOKE:
                play_smokepuff(at.position, at.rotation);
                break;
            case VFX_TYPE.MUZZLEFLASH:
                play_muzzleflash(at.position, at.rotation);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void play_smokepuff(Vector3 pos, Quaternion rot)
    {
        var smoke = Instantiate(_smokeBig);
        smoke.transform.SetPositionAndRotation(pos, rot);
        smoke.Play();
        Destroy(smoke.gameObject, 3);
    }

    public void play_muzzleflash(Vector3 pos, Quaternion rot)
    {
        var mz = Instantiate(_muzzleFlash);
        var smoke = Instantiate(_smokeBig);
        var smokeshort = Instantiate(_smokeGun);
        var sparks = Instantiate(_sparks);

        mz.transform.SetPositionAndRotation(pos, rot);
        smoke.transform.SetPositionAndRotation(pos, rot);
        smokeshort.transform.SetPositionAndRotation(pos, rot);
        sparks.transform.SetPositionAndRotation(pos, rot);

        mz.Play();
        smoke.Play();
        smokeshort.Play();
        sparks.Play();

        Destroy(mz.gameObject, 2);
        Destroy(smoke.gameObject, 3);
        Destroy(smokeshort.gameObject, 2);
        Destroy(sparks.gameObject, 2);
    }
    
    public void apply_force(Vector3 at, float force, float radius)
    {
        var colliders = Physics.OverlapSphere(at, radius);
        foreach (var col in colliders)
        {
            var rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, at, radius);
            }
        }
    }
}

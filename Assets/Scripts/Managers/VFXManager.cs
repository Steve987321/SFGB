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
    [SerializeField] private ParticleSystem _smoke;

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
        var smoke = Instantiate(_smoke);
        smoke.transform.SetPositionAndRotation(pos, rot);
        smoke.Play();
        Destroy(smoke.gameObject, smoke.main.duration);
    }

    public void play_muzzleflash(Vector3 pos, Quaternion rot)
    {
        var mz = Instantiate(_muzzleFlash);
        var smoke = Instantiate(_smoke);
        var sparks = Instantiate(_sparks);

        mz.transform.SetPositionAndRotation(pos, rot);
        smoke.transform.SetPositionAndRotation(pos, rot);
        sparks.transform.SetPositionAndRotation(pos, rot);

        mz.Play();
        smoke.Play();
        sparks.Play();

        Destroy(mz.gameObject, mz.main.duration);
        Destroy(smoke.gameObject, smoke.main.duration);
        Destroy(sparks.gameObject, sparks.main.duration);
    }
    
}

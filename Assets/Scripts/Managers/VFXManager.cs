using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.AI;
using Random = UnityEngine.Random;

public class VFXManager : MonoBehaviour
{
    public enum VFX_TYPE
    {
        SMOKE,
        MUZZLEFLASH,
        SPARKHITBIG,
        SPARKHIT,

        BLOODHIT,
    }

    public enum BULLET_HOLE_TYPE
    {
        GLASS,
        METAL,
        WOOD,
        EXPLOSION,
    }

    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private ParticleSystem _sparks;
    [SerializeField] private ParticleSystem _smokeGun;
    [SerializeField] private ParticleSystem _smokeBig;
    [SerializeField] private ParticleSystem _bigSparkHit;

    [Space] 
    
    [SerializeField] private Material[] _muzzleFlashMaterials;

    [Space] 

    [SerializeField] private GameObject[] _metalHits;
    [SerializeField] private GameObject[] _woodHits;
    [SerializeField] private GameObject[] _glassHits;

    [Space]
    
    [SerializeField] private ParticleSystem _bloodSplatter;
    [SerializeField] private ParticleSystem _bloodMist;

    private List<GameObject> _activeBulletHoles = new();

    public static VFXManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void play_FX(Vector3 pos, Quaternion rot, VFX_TYPE type)
    {
        switch (type)
        {
            case VFX_TYPE.SMOKE:
                play_smokepuff(pos, rot);
                break;
            case VFX_TYPE.MUZZLEFLASH:
                play_muzzleflash(pos, rot);
                break;
            case VFX_TYPE.SPARKHITBIG:
                play_sparkHitBig(pos, rot);
                break;
            case VFX_TYPE.BLOODHIT:
                play_bloodMist(pos, rot);
                play_bloodSplatter(pos, rot);
                break;
            case VFX_TYPE.SPARKHIT:
                play_sparkHit(pos, rot);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void play_FX(Transform at, VFX_TYPE type)
    {
        play_FX(at.position, at.rotation, type);
    }

    private void PlayParticleSystem(ParticleSystem p, Vector3 pos, Quaternion rot, float delay = 5)
    {
        var instance = Instantiate(p);
        instance.transform.SetPositionAndRotation(pos, rot);
        instance.Play();
        Destroy(instance.gameObject, delay);
    }

    public void play_bloodSplatter(Vector3 pos, Quaternion rot)
    {
        PlayParticleSystem(_bloodSplatter, pos, rot, 3f);
    }
    public void play_bloodMist(Vector3 pos, Quaternion rot)
    {
        PlayParticleSystem(_bloodMist, pos, rot, 4f);
    }

    public void play_smokepuff(Vector3 pos, Quaternion rot)
    {
        var main = _smokeBig.main;
        var col = main.startColor.color;
        main.startColor = new Color(col.r, col.g, col.b, Random.Range(0f, 0.36f));
        _smokeBig.GetComponent<ParticleSystemRenderer>().maxParticleSize = Random.Range(2f, 5f);
        main.startRotationZ = Random.Range(0f, 1f);
        PlayParticleSystem(_smokeBig, pos, rot, 3);
    }

    public void play_muzzleflash(Vector3 pos, Quaternion rot)
    {
        _muzzleFlash.GetComponent<Renderer>().material =
            _muzzleFlashMaterials[Random.Range(0, _muzzleFlashMaterials.Length)];
        play_smokepuff(pos, rot);
        PlayParticleSystem(_muzzleFlash, pos, rot, 3);
        PlayParticleSystem(_smokeGun, pos, rot, 3);
        PlayParticleSystem(_sparks, pos, rot, 3);
    }

    public void play_sparkHitBig(Vector3 pos, Quaternion rot)
    {
        PlayParticleSystem(_bigSparkHit, pos, rot, 3);
        PlayParticleSystem(_sparks, pos, rot, 3);
    }
    private void play_sparkHit(Vector3 pos, Quaternion rot)
    {
        PlayParticleSystem(_sparks, pos, rot, 3);
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
    public void apply_force(Vector3 at, float force, float radius, Rigidbody[] Exclude)
    {
        var colliders = Physics.OverlapSphere(at, radius);
        foreach (var col in colliders)
        {
            var rb = col.GetComponent<Rigidbody>();
            if (Exclude.Contains(rb)) continue;

            if (rb != null)
            {
                rb.AddExplosionForce(force, at, radius);
            }
        }
    }

    public void add_bullet_hole(Vector3 pos, Vector3 normal, BULLET_HOLE_TYPE type, Transform trans)
    { 
        var bHoleObj = type switch
        {
            BULLET_HOLE_TYPE.GLASS => _glassHits[Random.Range(0, _glassHits.Length)],
            BULLET_HOLE_TYPE.METAL => _metalHits[Random.Range(0, _metalHits.Length)],
            BULLET_HOLE_TYPE.WOOD => _woodHits[Random.Range(0, _woodHits.Length)],
            BULLET_HOLE_TYPE.EXPLOSION => _metalHits[Random.Range(0, _metalHits.Length)],
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        var rot = Quaternion.FromToRotation(Vector3.up, normal);
        var bHole = Instantiate(bHoleObj, pos + (normal * 0.01f), rot);
        bHole.transform.parent = trans
;
        play_FX(pos, rot, type == BULLET_HOLE_TYPE.EXPLOSION ? VFX_TYPE.SPARKHITBIG : VFX_TYPE.SPARKHIT);

        _activeBulletHoles.Add(
            bHole
        );
    }

    public void add_bullet_hole(RaycastHit hit, BULLET_HOLE_TYPE type)
    {
        add_bullet_hole(hit.point, hit.normal, type, hit.transform);
    }

    public void CleanBulletHoles()
    {
        foreach (var bullethole in _activeBulletHoles)
        {
            Destroy(bullethole);
        }
    }
    
}

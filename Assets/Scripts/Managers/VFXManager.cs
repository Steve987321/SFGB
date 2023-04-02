using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

using Random = UnityEngine.Random;

public class VFXManager : NetworkBehaviour
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

    #region VFX

    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private ParticleSystem _gunSparks;
    [SerializeField] private ParticleSystem _sparks;
    [SerializeField] private ParticleSystem _sparks2;
    [SerializeField] private ParticleSystem _smokeGun;
    [SerializeField] private ParticleSystem _smokeBig;
    [SerializeField] private ParticleSystem _bigSparkHit;

    [Space] 
    
    [SerializeField] private Material[] _muzzleFlashMaterials;

    [Space] 

    [SerializeField] private GameObject[] _metalHits;

    [Space]
    
    [SerializeField] private ParticleSystem _bloodSplatter;
    [SerializeField] private ParticleSystem _bloodMist;

    #endregion

    private GOPooler _bulletHolePool = new();

    #region PostProcessing

    [SerializeField] private Volume _vol;
    
    private Vignette _vignette;
    private float _ogVigIntensity = 0;
    private float _ogVigSmoothness = 0;

    private FilmGrain _grain;
    private float _ogGrainIntensity = 0;

    #endregion

    public static VFXManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _bulletHolePool.use_array = true;
        _bulletHolePool.pooledObjectArray = _metalHits;
        _bulletHolePool.PoolSize = 20;
        _bulletHolePool.CreatePool();

        if (_vol.profile.TryGet<Vignette>(out var vignette))
        {
            _vignette = vignette;
            _ogVigIntensity = vignette.intensity.value;
            _ogVigSmoothness = vignette.smoothness.value;
        }
        if (_vol.profile.TryGet<FilmGrain>(out var grain))
        {
            _grain = grain;
            _ogGrainIntensity = _grain.intensity.value;
        }
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

    private bool _vigCoroutineStarted = false;
    private float _vigCoroutineTimer = 0.0f;
    public void AddVignetteIntesity()
    {
        if (_vignette.intensity.value < 1f)
            _vignette.intensity.value += 0.05f;
        if (_vignette.smoothness.value < 1f)
            _vignette.smoothness.value += 0.1f;

        _vigCoroutineTimer = 1f; // delay a second before lerping back to og val
        if (!_vigCoroutineStarted)
            StartCoroutine(VignetteLerper());
    }


    public void SetFilmGrain(float val)
    {
        val = Mathf.Clamp01(val);
        _grain.intensity.value = val;
    }

    private System.Collections.IEnumerator VignetteLerper()
    {
        _vigCoroutineStarted = true;

        while (_vignette.intensity.value > _ogVigIntensity || _vignette.smoothness.value > _ogVigSmoothness)
        {
            _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, _ogVigIntensity, 2f * Time.deltaTime);
            _vignette.smoothness.value = Mathf.Lerp(_vignette.smoothness.value, _ogVigSmoothness, 2f * Time.deltaTime);

            if (_vigCoroutineTimer > 0)
                while (_vigCoroutineTimer > 0)
                {
                    _vigCoroutineTimer -= Time.deltaTime;
                    yield return null;
                }

            yield return null;
        }

        _vigCoroutineStarted = false;
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
        PlayParticleSystem(_gunSparks, pos, rot, 3);
    }

    public void play_sparkHitBig(Vector3 pos, Quaternion rot)
    {
        PlayParticleSystem(_bigSparkHit, pos, rot, 3);
        PlayParticleSystem(_sparks, pos, rot, 3);
    }
    private void play_sparkHit(Vector3 pos, Quaternion rot)
    {
        PlayParticleSystem(_sparks, pos, rot, 3);
        PlayParticleSystem(_sparks2, pos, rot, 3);
    }

    public void apply_force(Vector3 at, float force, float radius)
    {
        apply_forceServerRpc(at, force, radius);
    }

    [ServerRpc(RequireOwnership = false)]
    private void apply_forceServerRpc(Vector3 at, float force, float radius)
    {
        apply_forceClientRpc(at, force, radius);
    }

    [ClientRpc]
    private void apply_forceClientRpc(Vector3 at, float force, float radius)
    {
        var colliders = Physics.OverlapSphere(at, radius);
        foreach (var col in colliders)
        {
            var rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, at, radius, 1, ForceMode.Force);
            }
        }
    }

    private Rigidbody[] rbs = new Rigidbody[25];
    public void apply_forceEx(Vector3 at, float force, float radius, Rigidbody[] Exclude)
    {
        rbs = Exclude;
        apply_forceExServerRpc(at.x, at.y, at.z, force, radius);
    }

    [ServerRpc(RequireOwnership = false)]
    private void apply_forceExServerRpc(float x, float y, float z, float force, float radius)
    {
        apply_forceExClientRpc(x, y, z, force, radius);
    }

    [ClientRpc]
    void apply_forceExClientRpc(float x, float y, float z, float force, float radius)
    {
        var at = new Vector3(x, y, z);
        var colliders = Physics.OverlapSphere(at, radius);

        foreach (var col in colliders)
        {
            var rb = col.GetComponent<Rigidbody>();
            if (rbs.Contains(rb)) continue;

            if (rb != null)
            {
                rb.AddExplosionForce(force, at, radius, 1, ForceMode.Force);
            }
        }
    }

    public void add_bullet_hole(Vector3 pos, Vector3 normal, BULLET_HOLE_TYPE type, Transform trans)
    {
        var rot = Quaternion.FromToRotation(Vector3.up, normal);

        var bHole = _bulletHolePool.GetPooledObject();
        bHole.transform.SetPositionAndRotation(pos + normal * 0.01f, rot);
        bHole.transform.parent = trans;
        bHole.SetActive(true);
        play_FX(pos, rot, type == BULLET_HOLE_TYPE.EXPLOSION ? VFX_TYPE.SPARKHITBIG : VFX_TYPE.SPARKHIT);
    }

    public void add_bullet_hole(RaycastHit hit, BULLET_HOLE_TYPE type)
    {
        add_bullet_hole(hit.point, hit.normal, type, hit.transform);
    }

    /*
     * applies spherical damage to players within the radius
     */
    public void apply_radius_damage(Vector3 at, float radius, float damage)
    {
        // a copy from rocket.cs with different parameters
        var collisionRoots = new List<Transform>();

        foreach (var collider in Physics.OverlapSphere(at, radius))
            if (collider.transform.root.TryGetComponent<Player>(out var player)
                && !collisionRoots.Contains(collider.transform.root))
            {
                player.DoDamage(damage);
                collisionRoots.Add(collider.transform.root);
            }

        CameraManager.Instance.DoShake(0.5f, 2f, 5);
    }
    
}

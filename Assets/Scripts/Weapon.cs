using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : NetworkBehaviour
{
    public enum WEAPON
    {
        NONE,
        AK,
        SHOTGUN,
        M4,
        PISTOL,
        RPG,
        SMG
    }

    [System.Serializable]
    public struct gunFX
    {
        public VFXManager.VFX_TYPE Type;
        public Transform Transform;
    }

    [HideInInspector] public bool IsEquippedByPlayer;

    private bool _canShoot = false;

    [SerializeField] private Transform _endOfBarrel;

    [Tooltip("The FX that plays on shooting")]
    public gunFX[] GunFxs;

    public WEAPON WeaponType = WEAPON.NONE;

    [Space]

    [Header("set this when RPG is the weapon type")]
    [SerializeField] private GameObject RocketObj;

    [Header("Weapon Stats")]
    public float Recoil;
    public float FireDelay;
    public float Damage;
    public NetworkVariable<int> Ammo = new(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Tooltip("the force of impact")]
    public float BulletForce;

    private List<Transform> _rbInRoot = new List<Transform>();

    void Start()
    {
        if (GunFxs.Length == 0)
        {
            Debug.LogError("make sure atleast one FX is selected on the weapon component, from" + this.name);
        }
        if (transform.root.gameObject.name.Contains("player", StringComparison.OrdinalIgnoreCase))
            _rbInRoot.AddRange(Helper.GetAllTransformsInChildren(transform.root.gameObject).ToList());
        _canShoot = true;

        if (IsServer)
            Ammo.Value += 1;
    }

    public void Shoot()
    {
        ShootServerRpc();
    }

    private bool once = false;

    [ServerRpc(RequireOwnership = false)]
    public void ShootServerRpc()
    {
        if (!once)
        {
            Ammo.Value--;
            once = true;
        }
        ShootClientRpc();
    }

    [ClientRpc]
    public void ShootClientRpc()
    {
        if (_canShoot && Ammo.Value > 0)
        {
            StartCoroutine(_Shoot());
        }
    }

    private IEnumerator _Shoot()
    {
        _canShoot = false;

        // extra variation
        if (Random.Range(0f, 1f) > 0.1f)
            AudioManager.Instance.Play_BulletFlyBy();

        Debug.DrawRay(_endOfBarrel.position, _endOfBarrel.forward, Color.red, 3);
        bool rocketFlag = false;

        // Rocket has own hitscan function
        if (WeaponType == WEAPON.RPG)
        {
            AudioManager.Instance.PlayDeafningFX(0.2f);
            AudioManager.Instance.Play_RPGShoot(_endOfBarrel.position);

            if (Physics.Raycast(_endOfBarrel.position, _endOfBarrel.forward, out var hit))
            {
                if (Helper.IsInReach(hit.point, _endOfBarrel.position, 20))
                {
                    var relativePos = _endOfBarrel.position - hit.point;

                    Rocket.DoRocketDamage(transform.root, hit.point);
                    
                    VFXManager.Instance.apply_force(hit.point, 1200, 20);
                    VFXManager.Instance.add_bullet_hole(hit.point, hit.normal, VFXManager.BULLET_HOLE_TYPE.EXPLOSION, hit.transform.transform);
                    VFXManager.Instance.play_sparkHitBig(hit.point, Quaternion.LookRotation(relativePos, Vector3.up));

                    rocketFlag = true;
                }
            }

            var shotRocket = Instantiate(RocketObj, _endOfBarrel.position, RocketObj.transform.rotation);
            Destroy(RocketObj);
            
            if (!rocketFlag)
            {
                var rocket = shotRocket.AddComponent<Rocket>();
                rocket.PlayerTransform = transform;
                rocket.ExcludeObj.AddRange(_rbInRoot);
            }
            var rb = shotRocket.AddComponent<Rigidbody>();
            rb.mass = 0.5f;
            rb.useGravity = false;
            rb.AddForce(25 * -shotRocket.transform.forward, ForceMode.Impulse);
            if (rocketFlag)
            {
                Destroy(shotRocket, 1);
            }
            // destroy the rocket after some seconds in case nothing has been hit 
            //Destroy(rocket.gameObject, 7);
        }
        else
        {
            AudioManager.Instance.PlayDeafningFX(0.025f);
            AudioManager.Instance.Play_GunShoot(_endOfBarrel.position);
            if (Physics.Raycast(_endOfBarrel.position, _endOfBarrel.forward, out var hit))
            {
                if (Helper.IsInReach(hit.point, _endOfBarrel.position, 3f) || Helper.IsInReach(hit.point, Camera.main.transform.position, 5f))
                {
                    AudioManager.Instance.Play_BulletFlyBy();
                }
                AudioManager.Instance.Play_BulletHitServerRpc(hit.point);
                //print("hit: " + hit.collider.name);
                VFXManager.Instance.apply_force(hit.point, BulletForce * 100, 3);
                //VFXManager.Instance.play_FX(hit.point, hit.normal, );
                HitScan(hit);
            }
        }

        foreach (var t in GunFxs)
        {
            VFXManager.Instance.play_FX(t.Transform, t.Type);
        }

        // apply recoil
        VFXManager.Instance.apply_force(transform.position, Recoil * 100f, 2);

        // apply cam shake
        CameraManager.Instance.DoShake(0.2f, 3f, 1.5f);

        yield return new WaitForSeconds(FireDelay);
        if (WeaponType != WEAPON.RPG) _canShoot = true;
        once = false;
    }

    /// <summary>
    /// scans for hit for type
    /// </summary>
    /// <returns>whether a player has been hit</returns>
    private bool HitScan(RaycastHit hit)
    {
        var root = hit.transform.root;
        if (root.TryGetComponent<Player>(out var player))
        {
            var relativePos = hit.point - transform.position;
            VFXManager.Instance.play_FX(hit.point, Quaternion.LookRotation(relativePos), VFXManager.VFX_TYPE.BLOODHIT);
            AudioManager.Instance.UpFightMusic();
            player.DoDamage(Damage);
            return true;
        }

        VFXManager.Instance.add_bullet_hole(hit, VFXManager.BULLET_HOLE_TYPE.METAL);

        return false;
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        var spos = Camera.main.WorldToScreenPoint(transform.position);
        spos.z = 0;
        GUI.Label(new Rect(spos, new Vector2(100, 100)), Ammo.Value.ToString());
    }
#endif
}

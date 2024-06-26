using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// handles the weapons in the current scene
/// </summary>
public class WeaponManager : NetworkBehaviour
{
    // flag for when it is allowed to spawn in weapons
    public bool CanSpawnWeapons = false;

    // the interval to try and spawn weapons 
    public float WeaponSpawnFrequency = 15f;

    // when there are more weapons then this number it will not spawn anymore weapons
    public int MaxWeapons = 10;

    // what icon to show when showing loaded weapons in scene
    [SerializeField] private GameObject _icon;

    // weapon prefabs 
    [SerializeField] private GameObject[] _weapons;

    // will determine the location and bounds of where to spawn weapons
    [SerializeField] private Transform[] SpawnPlanes;

    // dynamic list of all the weapons that are in the scene
    private List<Weapon> _activeWeapons;

    private Transform _canvas;

    private float _timer = 0;

    private float _iconTimer = 0;

    void Awake()
    {
        if (!_icon.TryGetComponent<Image>(out _))
        {
            Debug.LogError("the Icon gameobject doesn't have image component!!");
        }

        _canvas = _icon.transform.root;
    }

    void Update()
    {
        _activeWeapons = FindObjectsOfType<Weapon>(false).ToList();

        // show where loaded guns are 
        if (Input.GetKeyDown(KeyCode.H) && _iconTimer <= 0)
        {
            foreach (var weapon in _activeWeapons)
            {
                if (weapon.CompareTag("Gun") && weapon.Ammo.Value > 1)
                {
                    var screenpoint = Camera.main.WorldToScreenPoint(weapon.transform.position);
                    var go = Instantiate(_icon, _canvas);
                    go.GetComponent<Image>().rectTransform.position = screenpoint;
                    go.GetComponent<Animator>().Play("GunIndicator");
                    Destroy(go, 1f);
                }
            }

            _iconTimer = 0.5f;
        }

        if (_iconTimer > 0)
            _iconTimer -= Time.deltaTime;

        if (!IsServer) return;

        if (!CanSpawnWeapons) return;

        if (_timer > WeaponSpawnFrequency)
        {
            CleanUpServerRpc();
            SpawnWeaponsServerRpc();
            _timer = 0;
        }

        _timer += Time.deltaTime;
    }

    [ServerRpc]
    private void SpawnWeaponsServerRpc()
    {
        SpawnWeapons();
    }

    private void SpawnWeapons()
    {
        if (_activeWeapons.Count >= MaxWeapons - 4) return;

        for (var i = 0; i < 4; i++)
        {
            int f = Random.Range(0, _weapons.Length);
            var weapon = Instantiate(_weapons[f]);
            weapon.GetComponent<NetworkObject>().Spawn(true);
            weapon.transform.position = Helper.GetRandomPointOnPlane(
                SpawnPlanes.Length == 1 ? SpawnPlanes[0] : SpawnPlanes[Random.Range(0, SpawnPlanes.Length)]
            );
        }
    }

    [ServerRpc]
    private void CleanUpServerRpc()
    {
        if (_activeWeapons.Count < MaxWeapons - 4) return;

        foreach (var weapon in _activeWeapons)
        {
            if (!weapon.IsEquippedByPlayer && weapon.gameObject.GetComponent<TransformFollower>().Target != null && weapon.Ammo.Value <= 1)
            {
                weapon.GetComponent<NetworkObject>().Despawn();
                Destroy(weapon.gameObject);
            }

            if (weapon.transform.position.y < -30)
            {
                weapon.GetComponent<NetworkObject>().Despawn();
                Destroy(weapon.gameObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// handles the weapons in the current scene
/// </summary>
public class WeaponManager : MonoBehaviour
{
    // flag for when it is allowed to spawn in weapons
    public bool CanSpawnWeapons = false;

    // the interval to try and spawn weapons 
    public float WeaponSpawnFrequency = 15f;

    // when there are more weapons then this number it will not spawn anymore weapons
    public int MaxWeapons = 10;

    // weapon prefabs 
    [SerializeField] private GameObject[] _weapons;

    // will determine the location and bounds of where to spawn weapons
    [SerializeField] private Transform SpawnPlane;

    // dynamic list of all the weapons that are in the scene
    private List<Weapon> _activeWeapons;

    private float _timer = 0;

    void Update()
    {
        _activeWeapons = FindObjectsOfType<Weapon>(false).ToList();
        if (!CanSpawnWeapons) return;

        if (_timer > WeaponSpawnFrequency)
        {
            CleanUp();
            SpawnWeapons();
            _timer = 0;
        }

        _timer += Time.deltaTime;
    }

    public void SpawnWeapons()
    {
        // spawn weapons in groups of four 
        if (_activeWeapons.Count >= MaxWeapons - 4) return;

        for (var i = 0; i < 4; i++)
        {
            int f = Random.Range(0, _weapons.Length);
            var weapon = Instantiate(_weapons[f]);
            weapon.transform.position = Helper.GetRandomPointOnPlane(SpawnPlane);
        }
    }

    private void CleanUp()
    {
        if (_activeWeapons.Count < MaxWeapons - 4) return;

        foreach (var weapon in _activeWeapons)
        {
            if (weapon.transform.parent == null && weapon.Ammo <= 0)
            {
                Destroy(weapon.gameObject);
            }
        }
    }
}

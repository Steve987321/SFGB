using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// will damage or destroy things that are entering the collision area
/// </summary>
public class KillZone : MonoBehaviour
{
    public float Damage = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent<Player>(out var player))
        {
            player.DoDamage(Damage);
        }

        if (other.transform.root.TryGetComponent<Weapon>(out var Weapon))
        {
            Destroy(other.transform.root.gameObject);
        }

    }
}

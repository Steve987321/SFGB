using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerResetZone : MonoBehaviour
{
    public float Damage = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NetworkObject>(out var no))
            no.Despawn();
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

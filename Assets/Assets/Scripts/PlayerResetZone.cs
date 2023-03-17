using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResetZone : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    public float ResetDamage = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent<Player>(out var player))
        {
            player.DoDamage(ResetDamage);
        }

    }
}

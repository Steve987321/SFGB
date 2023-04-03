using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TheReadyBox : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            player.SetReady(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            player.SetReady(false);
        }
    }
}

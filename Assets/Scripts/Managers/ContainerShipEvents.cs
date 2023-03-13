using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ContainerShipEvents : MonoBehaviour
{
    public float LightningChance = 10;

    [SerializeField] private ParticleSystem[] _waterSplash;
    [SerializeField] private Transform[] _waterSplashPoints;

    [SerializeField] private Transform _lightningBoltArea;
    [SerializeField] private GameObject _lightningBolt;

    void Start()
    {
        InvokeRepeating(nameof(LightningStrike), 1, 20f);
        InvokeRepeating(nameof(WaterSplash), 1, 7f);
    }

    /*
     * plays a lightning bolt on a random point on the container ship
     */
    void LightningStrike()
    {
        if (Random.Range(0, 100) < LightningChance)
        {
            StartCoroutine(PlayLightningStrike());
        }
    }

    IEnumerator PlayLightningStrike()
    {
        _lightningBolt.transform.position = Helper.GetRandomPointOnPlane(_lightningBoltArea);
        _lightningBolt.SetActive(true);

        yield return new WaitForSeconds(0.1f); // show bolt for 0.1 seconds

        _lightningBolt.SetActive(false);
    }

    /*
     * plays a water splash on random side of the container ship (front or back side)
     */
    void WaterSplash()
    {
        var point = _waterSplashPoints[Random.Range(0, _waterSplashPoints.Length)];
        foreach (var splash in _waterSplash)
        {
            var go = Instantiate(splash.gameObject);
            go.transform.SetPositionAndRotation(point.position, point.rotation);
            go.GetComponent<ParticleSystem>().Play();
            Destroy(go, 3f);
        }
    }
}

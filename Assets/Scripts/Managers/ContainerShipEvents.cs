using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ContainerShipEvents : MonoBehaviour
{

    [SerializeField] private ParticleSystem[] _waterSplashFX;
    [SerializeField] private Transform[] _waterSplashPoints;

    [Space]

    [Header("Lightning Settings")]
    public float LightningChance = 10;
    [Tooltip("How many times the lightning can show up, the lower the more frequent")]
    public float LightningShowDelay = 20f;
    [Tooltip("How long in seconds it should show warning before lightning impact")]
    public float LightningWarningTime = 2f;

    public float LightningBoltRadius = 5f;
    public float LightningBoltDamage = 10f;

    [SerializeField] private Transform _lightningBoltArea;
    [SerializeField] private GameObject _lightningBolt;
    [SerializeField] private GameObject _warning;
    [SerializeField] private TextMeshPro _countdownTimerObj;

    private float _countdownTimer = 0;

    void Start()
    {
        InvokeRepeating(nameof(LightningStrike), 1, LightningShowDelay);
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
        // show warning

        var randomPos = Helper.GetRandomPointOnPlane(_lightningBoltArea);
        var warningobj = Instantiate(_warning);
        warningobj.transform.SetPositionAndRotation(randomPos, Quaternion.identity);

        _countdownTimer = LightningWarningTime;

        var countdownObj = Instantiate(_countdownTimerObj);
        countdownObj.transform.SetPositionAndRotation(randomPos + new Vector3(0, 0.5f, 0), Quaternion.Euler(60, 0, 0));
        
        // give player time to move and show timer
        while (_countdownTimer >= 0)
        {
            var halfradius = LightningBoltRadius / 2;
            Debug.DrawLine(randomPos + new Vector3(-halfradius, -halfradius, -halfradius), randomPos + new Vector3(halfradius, halfradius, halfradius));

            countdownObj.text = _countdownTimer.ToString("N1");
            _countdownTimer -= Time.deltaTime;
            yield return null;
        }

        Destroy(warningobj);
        Destroy(countdownObj.gameObject);

        float randAngle = Random.Range(-20, 20);
        _lightningBolt.transform.SetPositionAndRotation(randomPos, Quaternion.Euler(randAngle, randAngle, randAngle));
        _lightningBolt.SetActive(true);

        VFXManager.Instance.apply_force(randomPos, 3000, LightningBoltRadius);   
        VFXManager.Instance.apply_radius_damage(randomPos, LightningBoltRadius, LightningBoltDamage);

        yield return new WaitForSeconds(0.15f); // show bolt for 0.15 seconds

        _lightningBolt.SetActive(false);
    }

    

    /*
     * plays a water splash on random side of the container ship (front or back side)
     */
    void WaterSplash()
    {
        var point = _waterSplashPoints[Random.Range(0, _waterSplashPoints.Length - 1)];
        foreach (var splash in _waterSplashFX)
        {
            var go = Instantiate(splash.gameObject);
            go.SetActive(true);
            go.transform.SetPositionAndRotation(point.position, point.rotation);
            go.GetComponent<ParticleSystem>().Play();
            Destroy(go, 4);
        }
    }
}

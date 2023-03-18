using System.Collections;
using TMPro;
using UnityEngine;

/*
 * handles the continous events on the container ship map
 */
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
    [SerializeField] private GameObject _warningObj;
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
        // show warning circle
        var randomPos = Helper.GetRandomPointOnPlane(_lightningBoltArea);
        _warningObj.SetActive(true);
        _warningObj.transform.SetPositionAndRotation(randomPos, Quaternion.identity);

        _countdownTimer = LightningWarningTime;

         _countdownTimerObj.gameObject.SetActive(true);
        _countdownTimerObj.transform.SetPositionAndRotation(randomPos + new Vector3(0, 0.5f, 0), Quaternion.Euler(60, 0, 0));
        
        // give player time to move and show timer
        while (_countdownTimer >= 0)
        {
            var halfradius = LightningBoltRadius / 2;
            Debug.DrawLine(randomPos + new Vector3(-halfradius, -halfradius, -halfradius), randomPos + new Vector3(halfradius, halfradius, halfradius));

            _countdownTimerObj.text = _countdownTimer.ToString("N1");
            _countdownTimer -= Time.deltaTime;
            yield return null;
        }

        _warningObj.SetActive(false);
        _countdownTimerObj.gameObject.SetActive(false);

        float randAngle = Random.Range(-20, 20);
        _lightningBolt.transform.SetPositionAndRotation(randomPos, Quaternion.Euler(randAngle, randAngle, randAngle));
        _lightningBolt.SetActive(true);

        AudioManager.Instance.PlayDeafningFX();
        AudioManager.Instance.Play_Thunder(randomPos);

        VFXManager.Instance.apply_force(randomPos, 3000, LightningBoltRadius);   
        VFXManager.Instance.apply_radius_damage(randomPos, LightningBoltRadius, LightningBoltDamage);
        VFXManager.Instance.play_sparkHitBig(randomPos, Quaternion.identity);

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

using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/*
 * handles the continous events on the container ship map
 */
public class ContainerShipEvents : NetworkBehaviour
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

    [Space]

    [SerializeField] private Transform _lightningBoltArea;
    [SerializeField] private GameObject _lightningBoltPrefab;
    [SerializeField] private GameObject _warningObjPrefab;
    [SerializeField] private TextMeshPro _countdownTimerObjPrefab;

    private NetworkVariable<float> _countdownTimer = new(0);

    private float _lightningCounter = 0;

    void Start()
    {
        InvokeRepeating(nameof(WaterSplash), 1, 7f);

        if (!IsServer) return;

        this.GetComponent<NetworkObject>().Spawn(true);

        _lightningCounter = LightningShowDelay;

        //InvokeRepeating(nameof(LightningStrikeServerRpc), 1, LightningShowDelay);
    }

    void Update()
    {
        if (!IsServer) return;

        if (_lightningCounter < 0)
        {
            LightingStrikeServerRpc();
            _lightningCounter = LightningShowDelay;
        }

        _lightningCounter -= Time.deltaTime;

    }

    private bool _flag = false;

    [ServerRpc]
    void LightingStrikeServerRpc()
    {
        if (!_flag)
        {
            _flag = !_flag;
            return;
        }
        if (Random.Range(0, 100) < LightningChance)
            StartCoroutine(PlayLightningStrike());
    }

    IEnumerator PlayLightningStrike()
    {
        // show warning circle
        var randomPos = Helper.GetRandomPointOnPlane(_lightningBoltArea);

        var warningObj = Instantiate(_warningObjPrefab);
        warningObj.transform.SetPositionAndRotation(randomPos, Quaternion.identity);
        warningObj.GetComponent<NetworkObject>().Spawn(true);

        _countdownTimer.Value = LightningWarningTime;

        //var countdownTimerObj = Instantiate(_countdownTimerObjPrefab);
        //countdownTimerObj.GetComponent<NetworkObject>().Spawn(true);
        //countdownTimerObj.transform.SetPositionAndRotation(randomPos + new Vector3(0, 0.5f, 0), Quaternion.Euler(60, 0, 0));
        
        // give player time to move and show timer
        while (_countdownTimer.Value >= 0)
        {
            var halfradius = LightningBoltRadius / 2;
            Debug.DrawLine(randomPos + new Vector3(-halfradius, -halfradius, -halfradius), randomPos + new Vector3(halfradius, halfradius, halfradius));

            //countdownTimerObj.text = _countdownTimer.Value.ToString("N1");
            _countdownTimer.Value -= Time.deltaTime;
            yield return null;
        }

        warningObj.GetComponent<NetworkObject>().Despawn();
        //countdownTimerObj.GetComponent<NetworkObject>().Despawn();

        float randAngle = Random.Range(-20, 20);
        var lightningBolt = Instantiate(_lightningBoltPrefab);
        lightningBolt.GetComponent<NetworkObject>().Spawn(true);
        lightningBolt.transform.SetPositionAndRotation(randomPos, Quaternion.Euler(randAngle, randAngle, randAngle));

        AudioManager.Instance.Play_Thunder(randomPos);

        VFXManager.Instance.apply_force(randomPos, 3000, LightningBoltRadius);   
        VFXManager.Instance.apply_radius_damage(randomPos, LightningBoltRadius, LightningBoltDamage);
        VFXManager.Instance.play_sparkHitBig(randomPos, Quaternion.identity);

        AudioManager.Instance.PlayDeafningFX(0.1f);

        yield return new WaitForSeconds(0.15f); // show bolt for 0.15 seconds

        lightningBolt.GetComponent<NetworkObject>().Despawn();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioSource _musicAudioSource;
    [Space]
    [SerializeField] private AudioClip _musicLoop1;
    [SerializeField] private AudioClip _musicLoop2;
    public float FightVolBoost = 0.2f;
    private float _ogMusicVol = 0.5f;

    [Header("Ambience")] 
    public bool UseAmbientSounds = false;
    public float AmbiencePlayDelay = 5f;
    [Tooltip("range 0 - 1")]
    public float AmbienceStereoRand = 0.5f;
    [SerializeField] private AudioSource _ambienceAudioSource;
    [Space]
    [SerializeField] private AudioClip[] _ambientSounds;

    [Header("Sfx")] 
    [SerializeField] private AudioSource _sfxAudioSource;
    private float _ogSfxVol = 0.5f;
    [Space]
    [SerializeField] private AudioClip[] _bulletImpactSounds;
    [SerializeField] private AudioClip[] _rpgImpactSounds;
    [SerializeField] private AudioClip[] _gunShootSounds;
    [SerializeField] private AudioClip[] _bulletFlyBySounds;
    [SerializeField] private AudioClip[] _gunClickSounds;
    [SerializeField] private AudioClip[] _swooshSounds;
    [SerializeField] private AudioClip[] _punchSounds;
    [SerializeField] private AudioClip _thunderExplosionSound;
    [SerializeField] private AudioClip _RPGAudioShootSound;

    [Space]

    public bool isInFight = false;

    private bool _isPlayingFightMusic = false;
    [HideInInspector] public float InFightTimer = 0;

    private float _swooshCooldownTimer = 0;

    private Pooler sfxPool = new();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (UseAmbientSounds)
        {
            InvokeRepeating(nameof(PlayAmbienceInvoke), 0, AmbiencePlayDelay);
        }

        _ogMusicVol = _musicAudioSource.volume;

        _musicAudioSource.Play();
        sfxPool.PoolSize = 5;
        sfxPool.pooledObject = _sfxAudioSource.gameObject;
        sfxPool.CreatePool();
    }
    
    void Update()
    {
        if (isInFight && !_isPlayingFightMusic)
        {
            _musicAudioSource.clip = _musicLoop2;
            _musicAudioSource.Play();
            _musicAudioSource.volume += FightVolBoost;
            _isPlayingFightMusic = true;
        }
        else if (!_musicAudioSource.isPlaying)
        {
            _musicAudioSource.volume = _ogMusicVol;
            _musicAudioSource.clip = _musicLoop1;
            _musicAudioSource.Play();
        }

        _isPlayingFightMusic = _musicAudioSource.clip == _musicLoop2;

        if (_swooshCooldownTimer > 0)
            _swooshCooldownTimer -= Time.deltaTime;

        if (InFightTimer > 0) 
            InFightTimer -= Time.deltaTime;
        else 
            isInFight = false;
    }

    private void PlayAmbienceInvoke()
    {
        _ambienceAudioSource.clip = _ambientSounds[Random.Range(0, _ambientSounds.Length)];
        _ambienceAudioSource.panStereo = Random.Range(0.5f - AmbienceStereoRand / 2, 0.5f + AmbienceStereoRand / 2);
        _ambienceAudioSource.Play();
    }

    private void Play_SFX(AudioClip clip, Vector3 at, bool is2d = false, float vol = -1.0f)
    {
        var audiosource = sfxPool.GetPooledObject().GetComponent<AudioSource>();
        audiosource.transform.position = at;
        audiosource.gameObject.SetActive(true);
        if (vol > 0.0f)
            audiosource.volume = vol;
        audiosource.clip = clip;
        audiosource.spatialBlend = is2d ? 0 : 1;
        audiosource.Play();
        StartCoroutine(ResetVolume(audiosource, audiosource.clip.length));
    }
    private void Play_SFX(AudioClip clip, bool is2d = false, float vol = -1.0f)
    {
        var audiosource = sfxPool.GetPooledObject().GetComponent<AudioSource>();
        audiosource.gameObject.SetActive(true);
        audiosource.clip = clip;
        if (vol > 0.0f)
            audiosource.volume = vol;
        audiosource.spatialBlend = is2d ? 0 : 1;
        audiosource.Play();
        StartCoroutine(ResetVolume(audiosource, audiosource.clip.length));
    }

    IEnumerator ResetVolume(AudioSource Asource, float length)
    {
        yield return new WaitForSeconds(length);
        Asource.volume = _ogSfxVol;
    }

    public void Play_BulletHit(Vector3 at)
    {
        Play_SFX(_bulletImpactSounds[Random.Range(0, _bulletImpactSounds.Length)], at, false, 0.2f);
    }

    public void Play_GunClick(Vector3 at)
    {
        Play_SFX(_gunClickSounds[Random.Range(0, _gunClickSounds.Length)], at);
    }

    public void Play_GunShoot(Vector3 at)
    {
        Play_SFX(_gunShootSounds[Random.Range(0, _gunShootSounds.Length)], at);
    }

    public void Play_RPGShoot(Vector3 at)
    {
        Play_SFX(_RPGAudioShootSound, at);
    }

    public void Play_Swoosh(Vector3 at)
    {
        if (_swooshCooldownTimer > 0) return;
        Play_SFX(_swooshSounds[Random.Range(0, _swooshSounds.Length)], at, true);
        _swooshCooldownTimer = 0.5f;
    }

    public void Play_Punch(Vector3 at)
    {
        Play_SFX(_punchSounds[Random.Range(0, _punchSounds.Length)], at);
    }

    public void Play_Explosion(Vector3 at)
    {
        Play_SFX(_rpgImpactSounds[Random.Range(0, _rpgImpactSounds.Length)], at);
    }

    public void Play_Thunder(Vector3 at)
    {
        Play_SFX(_thunderExplosionSound, at);
    }

    public void Play_BulletFlyBy()
    {
        Play_SFX(_bulletFlyBySounds[Random.Range(0, _bulletFlyBySounds.Length)]);
    }

    public void UpFightMusic()
    {
        isInFight = true;
        InFightTimer = 10f;
    }

}

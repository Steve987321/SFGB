using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Experimental.AI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer _audioMixer;

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

    #region AudioMixer
    
    //private float _ogFreqGain = 0;
    //private float _ogOctaveRange = 0;
    //private float _ogCenterFreq = 0;

    public float FreqGainLimit = 8000f;
    public float OctaveRangeLimit = 8000f;
    public float CenterFreqLimit = 8000f;

    #endregion

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

        CenterFreqLimit = GetParamEQ_CenterFreq();
        FreqGainLimit = GetParamEQ_FreqGain();
        OctaveRangeLimit = GetParamEQ_OctaveRange();

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

    public void SetParamEQ(float centerFreq, float octaveRange, float freqGain)
    {
        _audioMixer.SetFloat("PARAMEQ_CENTERFREQ", centerFreq);
        _audioMixer.SetFloat("PARAMEQ_OCTAVERANGE", octaveRange);
        _audioMixer.SetFloat("PARAMEQ_FREQGAIN", freqGain);
    }

    public float GetParamEQ_CenterFreq()
    {
        if (_audioMixer.GetFloat("PARAMEQ_CENTERFREQ", out var tmp))
           return tmp;
        return 0;
    }
    public float GetParamEQ_OctaveRange()
    {
        if (_audioMixer.GetFloat("PARAMEQ_OCTAVERANGE", out var tmp))
           return tmp;
        return 0;
    }
    public float GetParamEQ_FreqGain()
    {
        if (_audioMixer.GetFloat("PARAMEQ_FREQGAIN", out var tmp))
           return tmp;
        return 0;
    }

    public void SetDistortion(float distortion)
    {
        _audioMixer.SetFloat("DISTORTION_LVL", distortion);
    }

    private float _deafenTimer = 0;
    private bool _deafening = false;
    public void PlayDeafningFX(float intensity = 0.5f)
    {
        _deafenTimer = 1;
        SetParamEQ(
            Mathf.Lerp(GetParamEQ_CenterFreq(), 7600f, intensity),
            Mathf.Lerp(GetParamEQ_OctaveRange(), 3f, intensity),
            Mathf.Lerp(GetParamEQ_FreqGain(), 0.1f, intensity)
        );

        if (!_deafening)
           StartCoroutine(deafeningLerp());
    }

    IEnumerator deafeningLerp()
    {
        _deafening = true;

        // lerp back very slowly
        while (GetParamEQ_CenterFreq() < CenterFreqLimit - 0.01f || GetParamEQ_OctaveRange() > OctaveRangeLimit + 0.01f || GetParamEQ_FreqGain() < FreqGainLimit - 0.01f)
        {
            SetParamEQ(
                Mathf.Lerp(GetParamEQ_CenterFreq(), CenterFreqLimit, 0.5f * Time.deltaTime),
                Mathf.Lerp(GetParamEQ_OctaveRange(), OctaveRangeLimit, 0.5f * Time.deltaTime),
                Mathf.Lerp(GetParamEQ_FreqGain(), FreqGainLimit, 0.5f * Time.deltaTime)
            );

            if (_deafenTimer > 0)
                while (_deafenTimer > 0)
                {
                    _deafenTimer -= Time.deltaTime;
                    yield return null;
                }
            yield return null;
        }

        _deafening = false;
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

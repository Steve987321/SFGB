using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public float ShakeAmplitude = 1.0f;
    public float ShakeSpeed = 1.0f;

    private Camera _camera;

    private float _shakeTimer = 0;
    private Quaternion _ogRot;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _camera = Camera.main;
        _ogRot = _camera.transform.rotation;
    }

    void Update()
    {
        if (_shakeTimer > 0)
        {
            var offset = Quaternion.Euler(
                Random.Range(-1f, 1f) * ShakeAmplitude,
                Random.Range(-1f, 1f) * ShakeAmplitude,
                Random.Range(-1f, 1f) * ShakeAmplitude
            );

            _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, _camera.transform.rotation * offset, Time.deltaTime * ShakeSpeed);
            _shakeTimer -= Time.deltaTime;
        }
        else
        {
            _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, _ogRot, Time.deltaTime * ShakeSpeed); ;
        }

    }

    /// <summary>
    /// Shakes the main camera.
    /// </summary>
    /// <param name="duration"> shake length in seconds </param>
    /// <param name="amp"> the intensity of the shake (range) </param>
    /// <param name="speed"> the speed for it to change rotation </param>
    public void DoShake(float duration, float amp = 1, float speed = 1)
    {
        _shakeTimer = duration;
        ShakeAmplitude = amp;
        ShakeSpeed = speed;
    }

}
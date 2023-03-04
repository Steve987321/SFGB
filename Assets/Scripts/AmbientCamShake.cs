using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class AmbientCamShake : MonoBehaviour
{
    public float ShakeSpeed = 1;
    public float ShakeAmp = 1;

    private Quaternion _ogRot;
    void Start()
    {
        _ogRot = transform.rotation;
    }

    void Update()
    {
        float elapsedTime = Time.time * ShakeSpeed;
        float x = Mathf.PerlinNoise(elapsedTime, 0) * 2 - 1;
        float y = Mathf.PerlinNoise(0, elapsedTime) * 2 - 1;

        var offset = Quaternion.Euler(
            x * ShakeAmp,
            y * ShakeAmp,
        x * ShakeAmp
        );

        transform.rotation = Quaternion.Lerp(transform.rotation, _ogRot * offset, Time.deltaTime * ShakeSpeed);
    }
}

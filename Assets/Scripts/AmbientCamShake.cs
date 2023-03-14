using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class AmbientCamShake : MonoBehaviour
{
    public float ShakeSpeed = 1;
    public float ShakeAmp = 1;

    public bool ShakePosition = false;
    public float PosShakeSpeed = 1;
    public float PosShakeAmp = 1;

    private Quaternion _ogRot;
    private Vector3 _ogPos;

    void Start()
    {
        _ogRot = transform.rotation;
        _ogPos = transform.position;
    }

    void Update()
    {
        var elapsedTime = Time.time * ShakeSpeed;
        var x = Mathf.PerlinNoise(elapsedTime, 0) * 2 - 1;
        var y = Mathf.PerlinNoise(0, elapsedTime) * 2 - 1;

        var offset = Quaternion.Euler(
            x * ShakeAmp,
            y * ShakeAmp,
            x * ShakeAmp
        );

        transform.rotation = Quaternion.Lerp(transform.rotation, _ogRot * offset, Time.deltaTime * ShakeSpeed);
        
        if (!ShakePosition) return;

        var rPosX = Mathf.PerlinNoise(elapsedTime, 0) * 2 - 1;
        var rPosY = Mathf.PerlinNoise(0, elapsedTime) * 2 - 1;
        var posOffset = new Vector3(
            rPosX * PosShakeAmp,
            rPosY * PosShakeAmp,
            rPosX * PosShakeAmp
        );

        transform.position = Vector3.Lerp(transform.position, _ogPos + posOffset, Time.deltaTime * PosShakeSpeed);
    }
}

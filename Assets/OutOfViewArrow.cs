using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to a 3d gameobject active in the scene that usually moves a lot.
/// if attached to gameobject it will show an arrow aiming at the gameobject
/// upon leaving the screen / the main camera.
/// </summary>
public class OutOfViewArrow : MonoBehaviour
{
    [HideInInspector]
    public bool IsOutOfView = false;

    [SerializeField] private RectTransform _arrow;

    void LateUpdate()
    {
        var targetPos2d = Camera.main.WorldToScreenPoint(transform.position) - _arrow.transform.position;
        Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, targetPos2d);
        _arrow.rotation = targetRot;
        //_arrow.localEulerAngles = Quaternion.LookRotation(targetPos2d).eulerAngles;
    }

}

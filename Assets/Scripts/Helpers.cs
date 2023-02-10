using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Helper
{
    public static bool IsInReach(Vector3 a, Vector3 b, float dist)
    {
        return (a - b).sqrMagnitude < dist * dist;
    }

    public static Transform GetClosest(Transform a, Transform[] list)
    {
        Transform bestTarget = null;
        var closestDistanceSqr = Mathf.Infinity;
        var currentPosition = a.position;
        foreach (var t in list)
        {
            var directionToTarget = t.position - currentPosition;
            var dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = t;
            }
        }

        return bestTarget;
    }
}

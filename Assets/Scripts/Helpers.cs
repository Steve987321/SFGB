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

    public static Rigidbody[] GetAllRigidBodiesInChildren(GameObject root, string Ignore = "")
    {
        var children = root.GetComponentsInChildren<Transform>();
        var rigidbodies = new Rigidbody[children.Length];
        int count = 0;

        foreach (var child in children)
        {
            if (Ignore != "" && child.name.Contains(Ignore)) continue;
            var rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rigidbodies[count] = rb;
                count++;
            }
        }

        // Trim array to actual number of rigidbodies
        System.Array.Resize(ref rigidbodies, count);

        return rigidbodies;
    }
    public static Transform[] GetAllTransformsInChildren(GameObject root, string Ignore = "")
    {
        var children = root.GetComponentsInChildren<Transform>();
        var transforms = new Transform[children.Length];
        int count = 0;

        foreach (var child in children)
        {
            if (Ignore != "" && child.name.Contains(Ignore)) continue;
            var rb = child.GetComponent<Transform>();
            if (rb != null)
            {
                transforms[count] = rb;
                count++;
            }
        }

        // Trim array to actual number of rigidbodies
        System.Array.Resize(ref transforms, count);

        return transforms;
    }
}

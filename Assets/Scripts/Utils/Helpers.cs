using System;
using System.Collections.Generic;
using UnityEngine;

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
        Array.Resize(ref rigidbodies, count);

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
        Array.Resize(ref transforms, count);

        return transforms;
    }

    /// <returns> A random point on the plane. </returns>
    public static Vector3 GetRandomPointOnPlane(Transform plane)
    {
        var x3 = plane.localScale.x * 2f;
        var z3 = plane.localScale.z * 2f;

        var x = UnityEngine.Random.Range(-x3, x3);
        var z = UnityEngine.Random.Range(-z3, z3);

        var randomPoint = new Vector3(plane.position.x + x, plane.position.y, plane.position.z + z);

        return randomPoint;
    }

    /// <summary>
    /// Swaps 2 elements in a list
    /// </summary>
    public static void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        (list[indexB], list[indexA]) = (list[indexA], list[indexB]);
    }

    /// <summary>
    /// Sets all child gameobjects of transform to layer,
    /// NOTE: doesn't set the root gameobject to layer.
    /// </summary>
    /// <param name="layer"> Layer child objects get sets to. </param>
    /// <param name="transform"> The root that contains child objects. </param>
    /// <param name="recursion"> Whether to use recursion on the child objects. </param>
    public static void SetChildLayers(LayerMask layer, Transform transform, bool recursion = false)
    {
        foreach (Transform t in transform)
        {
            t.gameObject.layer = layer;
            if (recursion)
                SetChildLayers(layer, t, true);
        }
    }

    /// <summary>
    /// Iterate through a transform root.
    /// NOTE: doesn't run function to the root gameobject.
    /// </summary>
    /// <param name="transform"> The root transform. </param>
    /// <param name="f"> A function that runs with the iterator transform as parameter. </param>
    /// <param name="recursion"> Whether to use recursion on the iterator. </param>
    public static void IterateThroughTransform(Transform transform, Action<Transform> f, bool recursion = false)
    {
        foreach (Transform t in transform)
        {
            f(t);
            if (recursion)
                IterateThroughTransform(t, f, true);
        }
    }
}

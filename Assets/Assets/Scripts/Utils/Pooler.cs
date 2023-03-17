using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Pooler : MonoBehaviour
{
    public GameObject pooledObject;
    public GameObject[] pooledObjectArray;

    public bool use_array = false;

    public int PoolSize;
    public List<GameObject> objectPool;

    public void CreatePool()
    {
        objectPool = new List<GameObject>();

        for (int i = 0; i < PoolSize; i++)
        {
            var obj = Instantiate(use_array ? pooledObjectArray[Random.Range(0, pooledObjectArray.Length)]: pooledObject);
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        foreach (var t in objectPool)
        {
            if (!t.activeInHierarchy)
                return t;
        }
        var res = objectPool[0];
        objectPool.RemoveAt(0);
        objectPool.Insert(objectPool.Count, res);
        return res;
    }

}

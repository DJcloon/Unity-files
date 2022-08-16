using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;
    public List<GameObject> pooledTorpedos;
    public List<GameObject> pooledDepthCharges;
    public GameObject torpedosToPool;
    public GameObject depthChargesToPool;
    public int amountToPool;

    void Awake()
    {
        SharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Loop through list of pooled objects,deactivating them and adding them to the list 
        pooledTorpedos = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(torpedosToPool);
            obj.SetActive(false);
            pooledTorpedos.Add(obj);
        }
        pooledDepthCharges = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(depthChargesToPool);
            obj.SetActive(false);
            pooledDepthCharges.Add(obj);
        }
    }

    public GameObject GetPooledTorpedos()
    {
        // For as many objects as are in the pooledObjects list
        for (int i = 0; i < pooledTorpedos.Count; i++)
        {
            // if the pooled objects is NOT active, return that object 
            if (!pooledTorpedos[i].activeInHierarchy)
            {
                return pooledTorpedos[i];
            }
        }
        // otherwise, return null   
        return null;
    }
    public GameObject GetPooledDepthCharges()
    {
        // For as many objects as are in the pooledObjects list
        for (int i = 0; i < pooledDepthCharges.Count; i++)
        {
            // if the pooled objects is NOT active, return that object 
            if (!pooledDepthCharges[i].activeInHierarchy)
            {
                return pooledDepthCharges[i];
            }
        }
        // otherwise, return null   
        return null;
    }

}

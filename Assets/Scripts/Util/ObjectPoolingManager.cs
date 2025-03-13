using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class ObjectPoolingManager : MonoBehaviour
{
    public static ObjectPoolingManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private Dictionary<string, object> _pools = new();

    // Method to get or create an object pool for a specific prefab
    public ObjectPool<T> GetOrCreatePool<T>(T prefab, int defaultCapacity = 10, int maxSize = 100) where T : Component
    {
        string key = typeof(T).Name + "_" + prefab.name;

        // Check if a pool already exists
        if (_pools.TryGetValue(key, out var existingPool))
            return (ObjectPool<T>)existingPool;

        // Create a new object pool
        var pool = new ObjectPool<T>(
            createFunc: () =>
            {
                T newObj = Instantiate(prefab);
                return newObj;
            },
            actionOnGet: obj =>
            {
                obj.gameObject.SetActive(true);
                if (obj is Projectile projectile)
                {
                    projectile.ResetState();
                }
            },
            actionOnRelease: obj => obj.gameObject.SetActive(false),
            actionOnDestroy: obj => Destroy(obj.gameObject),
            collectionCheck: false,
            defaultCapacity,
            maxSize
        );

        // Store the new pool in the dictionary for future use
        _pools[key] = pool;
        return pool;
    }

    // Overloaded method for GameObject pooling
    public ObjectPool<GameObject> GetOrCreatePool(GameObject prefab, int defaultCapacity = 10, int maxSize = 100)
    {
        string key = "GameObject_" + prefab.name;

        // Check if a pool already exists
        if (_pools.TryGetValue(key, out var existingPool))
            return (ObjectPool<GameObject>)existingPool;
      
        // Create a new object pool
        var pool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject newObj = Instantiate(prefab);
                return newObj;
            },
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: false,
            defaultCapacity,
            maxSize
        );

        // Store the pool in the dictionary for future retrieval
        _pools[key] = pool;
        return pool;
    }
}

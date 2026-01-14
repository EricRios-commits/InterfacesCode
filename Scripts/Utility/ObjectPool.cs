using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Simple object pool implementation for GameObjects.
    /// Manages a pool of inactive objects that can be reused.
    /// </summary>
    public class ObjectPool
    {
        private List<GameObject> pooledObjects;
        public GameObject Prefab { get; private set; }
        
        private int poolSize = 10;
        public int ActiveObjectsCount => pooledObjects.FindAll(x => x.activeInHierarchy).Count;
        
        public ObjectPool() {}
        
        public ObjectPool(GameObject objectToPool, int amountToPool)
        {
            CreatePool(objectToPool, amountToPool);
        }

        private void CreatePool(GameObject objectToPool, int amountToPool)
        {
            pooledObjects = new List<GameObject>();
            poolSize = amountToPool;
            Prefab = objectToPool;
            for (int i = 0; i < amountToPool; i++)
            {
                var tmp = Object.Instantiate(objectToPool);
                tmp.SetActive(false);
                pooledObjects.Add(tmp);
            }
        }

        private void AddMoreObjects(int amountToPool)
        {
            for (int i = 0; i < amountToPool; i++)
            {
                var tmp = Object.Instantiate(Prefab);
                tmp.SetActive(false);
                pooledObjects.Add(tmp);
            }
        }
        
        /// <summary>
        /// Gets an inactive object from the pool and activates it.
        /// Returns null if no objects are available.
        /// </summary>
        public GameObject GetPooledObject()
        {
            EnsurePoolInitialized();
            foreach (var obj in pooledObjects)
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }
            return null;
        }
        
        private void EnsurePoolInitialized()
        {
            if (pooledObjects.Any(pooledObject => pooledObject == null))
            {
                pooledObjects.RemoveAll(pooledObject => pooledObject == null);
                pooledObjects = new List<GameObject>();
                AddMoreObjects(poolSize);
            }
        }
    }
}


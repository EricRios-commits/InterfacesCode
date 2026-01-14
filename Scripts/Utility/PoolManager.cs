using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Static pool manager that manages multiple object pools.
    /// Automatically creates pools on first request for each prefab.
    /// </summary>
    public class PoolManager
    {
        private static PoolManager _instance;
        private static List<ObjectPool> _pools;
        
        /// <summary>
        /// Gets an object from the pool for the specified prefab.
        /// Creates a new pool if one doesn't exist for this prefab.
        /// </summary>
        public static GameObject GetObjectOfType(GameObject prefab, int amountToPool = 10)
        {
            if (_instance == null)
            {
                _instance = new PoolManager();
                _pools = new List<ObjectPool>();
            }
            
            foreach (var pool in _pools)
            {
                if (pool.Prefab == prefab)
                {
                    return pool.GetPooledObject();
                }
            }
            
            var newPool = new ObjectPool(prefab, amountToPool);
            _pools.Add(newPool);
            return newPool.GetPooledObject();
        }
        
        /// <summary>
        /// Gets the pool for the specified prefab.
        /// Creates a new pool if one doesn't exist.
        /// </summary>
        public static ObjectPool GetPool(GameObject prefab, int amountToPool = 10)
        {
            if (_instance == null)
            {
                _instance = new PoolManager();
                _pools = new List<ObjectPool>();
            }
            
            foreach (var pool in _pools)
            {
                if (pool.Prefab == prefab)
                {
                    return pool;
                }
            }
            
            var newPool = new ObjectPool(prefab, amountToPool);
            _pools.Add(newPool);
            return newPool;
        }
    }
}


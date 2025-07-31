using System.Collections.Generic;
using UnityEngine;

namespace WaveSystem.Utilities
{
    [DefaultExecutionOrder(-2)]
    public class ObjectPool : MonoBehaviour
    {
        // Singleton instance of the ObjectPool
        public static ObjectPool Instance { get; private set; }

        private GameObject _poolParent;
        // Dictionary to store separate pools for each prefab type
        private Dictionary<GameObject, Queue<GameObject>> _objectPools = new();
        private Dictionary<GameObject, GameObject> _prefabLookup = new();

        private void Awake()
        {
            if (Instance != this && Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void InitializePool()
        {
            _poolParent = new GameObject("PoolGameObjects");
            _poolParent.transform.SetParent(transform);
        }

        public void PreWarm(GameObject prefab, int count)
        {
            // Create pool for this prefab if it doesn't exist
            if (!_objectPools.ContainsKey(prefab))
            {
                _objectPools[prefab] = new Queue<GameObject>();
            }

            // Create parent folder for this prefab type
            GameObject prefabPoolParent = new GameObject($"Pool_{prefab.name}");
            prefabPoolParent.transform.SetParent(_poolParent.transform);

            for (int i = 0; i < count; i++)
            {
                GameObject obj = GameObject.Instantiate(prefab, prefabPoolParent.transform);
                obj.SetActive(false);
                
                // Store which prefab this instance came from
                _prefabLookup[obj] = prefab;
                _objectPools[prefab].Enqueue(obj);
            }
        }

        public GameObject Instantiate(GameObject prefab, Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
        {
            GameObject obj;

            // Check if we have a pool for this prefab type and if it has objects
            if (_objectPools.ContainsKey(prefab) && _objectPools[prefab].Count > 0)
            {
                obj = _objectPools[prefab].Dequeue();
                obj.SetActive(true);
            }
            else
            {
                // Create new instance if no pooled objects available
                obj = GameObject.Instantiate(prefab, parent);
                
                // Store which prefab this instance came from
                _prefabLookup[obj] = prefab;
                
                // If no pool exists, create one
                if (!_objectPools.ContainsKey(prefab))
                {
                    _objectPools[prefab] = new Queue<GameObject>();
                }
            }

            obj.transform.SetPositionAndRotation(position ?? Vector3.zero, rotation ?? Quaternion.identity);

            // Set parent if specified
            if (parent != null)
            {
                obj.transform.SetParent(parent);
            }

            return obj;
        }

        public void ReturnToPool(GameObject obj)
        {
            if (obj != null && _prefabLookup.ContainsKey(obj))
            {
                obj.SetActive(false);
                obj.transform.SetParent(_poolParent.transform);
                
                // Return to the correct pool based on original prefab
                GameObject originalPrefab = _prefabLookup[obj];
                _objectPools[originalPrefab].Enqueue(obj);
            }
        }
    }
}

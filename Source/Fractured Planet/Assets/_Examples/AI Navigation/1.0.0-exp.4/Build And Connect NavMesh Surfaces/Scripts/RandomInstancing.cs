using UnityEngine;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Unity.AI.Navigation.Samples
{
    /// <summary>
    /// Fill 5x5 tiles around the local position procedurally by instantiating prefabs at random positions/orientations
    /// </summary>
    [DefaultExecutionOrder(-200)]
    public class RandomInstancing : MonoBehaviour
    {
        public GameObject m_Prefab;
        public int m_PoolSize = 250;
        public int m_InstancesPerTile = 10;
        public bool m_RandomPosition = true;
        public bool m_RandomOrientation = true;
        public float m_Height;

        public int m_BaseHash = 347652783;
        public float m_Size = 100.0f;

        private List<Transform> _instances = new();
        private int _used;
        private int _locX, _locZ;

        private void Awake()
        {
            for (var i = 0; i < m_PoolSize; ++i)
            {
                var go = Instantiate(m_Prefab, Vector3.zero, Quaternion.identity);
                go.SetActive(false);
                _instances.Add(go.transform);
            }
        }

        private void OnEnable()
        {
            _locX = ~0;
            _locZ = ~0;
            UpdateInstances();
        }

        private void OnDestroy()
        {
            for (var i = 0; i < _instances.Count; ++i)
            {
                if (_instances[i])
                {
                    Destroy(_instances[i].gameObject);
                }
            }
            _instances.Clear();
        }

        private void Update()
        {
            UpdateInstances();
        }

        private void UpdateInstances()
        {
            var transformPosition = transform.position;
            var x = (int)Mathf.Floor(transformPosition.x / m_Size);
            var z = (int)Mathf.Floor(transformPosition.z / m_Size);
            if (x == _locX && z == _locZ)
            {
                return;
            }

            _locX = x;
            _locZ = z;

            _used = 0;
            for (var i = x - 2; i <= x + 2; ++i)
            {
                for (var j = z - 2; j <= z + 2; ++j)
                {
                    var count = UpdateTileInstances(i, j);
                    if (count != m_InstancesPerTile)
                    {
                        return;
                    }
                }
            }

            // Deactivate the remaining active elements in the pool.
            // Here we assume all active elements are contiguous and first in the list.
            for (var i = _used; i < m_PoolSize && _instances[i].gameObject.activeSelf; ++i)
                _instances[i].gameObject.SetActive(false);
        }

        private int UpdateTileInstances(int i, int j)
        {
            var seed = Hash2(i, j) ^ m_BaseHash;
            var count = System.Math.Min(m_InstancesPerTile, m_PoolSize - _used);
            for (var end = _used + count; _used < end; ++_used)
            {
                float x = 0;
                float y = 0;

                if (m_RandomPosition)
                {
                    x = Random(ref seed);
                    y = Random(ref seed);
                }
                var pos = new Vector3((i + x) * m_Size, m_Height, (j + y) * m_Size);

                if (m_RandomOrientation)
                {
                    var r = 360.0f * Random(ref seed);
                    _instances[_used].rotation = Quaternion.AngleAxis(r, Vector3.up);
                }
                _instances[_used].position = pos;
                _instances[_used].gameObject.SetActive(true);
            }

            if (count < m_InstancesPerTile)
            {
                Debug.LogWarning("Pool exhausted", this);
            }

            return count;
        }

        private static int Hash2(int i, int j)
        {
            return (i * 73856093) ^ (j * 19349663);
        }

        private static float Random(ref int seed)
        {
            seed = (seed ^ 123459876);
            var k = seed / 127773;
            seed = 16807 * (seed - k * 127773) - 2836 * k;
            if (seed < 0)
            {
                seed = seed + 2147483647;
            }
            var ran0 = seed * 1.0f / 2147483647.0f;
            seed = (seed ^ 123459876);
            return ran0;
        }
    }
}

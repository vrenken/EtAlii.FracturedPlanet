using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

// ReSharper disable once CheckNamespace
namespace Unity.AI.Navigation.Samples
{
    /// <summary>
    /// Build and update a localized navmesh from the sources marked by NavMeshSourceTag
    /// </summary>
    [DefaultExecutionOrder(-102)]
    public class LocalNavMeshBuilder : MonoBehaviour
    {
        /// <summary>
        /// The center of the build
        /// </summary>
        public Transform m_Tracked;

        /// <summary>
        /// The size of the build bounds
        /// </summary>
        public Vector3 m_Size = new(80.0f, 20.0f, 80.0f);

        private NavMeshData _navMesh;
        private AsyncOperation _operation;
        private NavMeshDataInstance _instance;
        private List<NavMeshBuildSource> _sources = new();

        private IEnumerator Start()
        {
            while (true)
            {
                UpdateNavMesh(true);
                yield return _operation;
            }
        }

        private void OnEnable()
        {
            // Construct and add navmesh
            _navMesh = new NavMeshData();
            _instance = NavMesh.AddNavMeshData(_navMesh);
            if (m_Tracked == null)
                m_Tracked = transform;
            UpdateNavMesh();
        }

        private void OnDisable()
        {
            // Unload navmesh and clear handle
            _instance.Remove();
        }

        private void UpdateNavMesh(bool asyncUpdate = false)
        {
            NavMeshSourceTag.Collect(ref _sources);
            var defaultBuildSettings = NavMesh.GetSettingsByID(0);
            var bounds = QuantizedBounds();

            if (asyncUpdate)
                _operation = NavMeshBuilder.UpdateNavMeshDataAsync(_navMesh, defaultBuildSettings, _sources, bounds);
            else
                NavMeshBuilder.UpdateNavMeshData(_navMesh, defaultBuildSettings, _sources, bounds);
        }

        private static Vector3 Quantize(Vector3 v, Vector3 quant)
        {
            var x = quant.x * Mathf.Floor(v.x / quant.x);
            var y = quant.y * Mathf.Floor(v.y / quant.y);
            var z = quant.z * Mathf.Floor(v.z / quant.z);
            return new Vector3(x, y, z);
        }

        private Bounds QuantizedBounds()
        {
            // Quantize the bounds to update only when theres a 10% change in size
            var center = m_Tracked ? m_Tracked.position : transform.position;
            return new Bounds(Quantize(center, 0.1f * m_Size), m_Size);
        }

        private void OnDrawGizmosSelected()
        {
            if (_navMesh)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(_navMesh.sourceBounds.center, _navMesh.sourceBounds.size);
            }

            Gizmos.color = Color.yellow;
            var bounds = QuantizedBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            Gizmos.color = Color.green;
            var center = m_Tracked ? m_Tracked.position : transform.position;
            Gizmos.DrawWireCube(center, m_Size);
        }
    }
}

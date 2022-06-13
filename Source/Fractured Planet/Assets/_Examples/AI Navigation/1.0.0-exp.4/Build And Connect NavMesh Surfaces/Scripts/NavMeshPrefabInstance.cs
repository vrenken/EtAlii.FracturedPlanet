using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

// ReSharper disable once CheckNamespace
namespace Unity.AI.Navigation.Samples
{
    /// <summary>
    /// Associate existing NavMesh data to a prefab
    /// </summary>
    [ExecuteInEditMode]
    [DefaultExecutionOrder(-102)]
    public class NavMeshPrefabInstance : MonoBehaviour
    {
        public NavMeshData navMeshData { get => m_NavMesh; set => m_NavMesh = value; }
        [SerializeField] private NavMeshData m_NavMesh;

        public bool followTransform { get => m_FollowTransform; set => SetFollowTransform(value); }
        [SerializeField] private bool m_FollowTransform;

        private NavMeshDataInstance m_Instance;

        // Position Tracking
        private static readonly List<NavMeshPrefabInstance> s_TrackedInstances = new List<NavMeshPrefabInstance>();
        public static List<NavMeshPrefabInstance> trackedInstances => s_TrackedInstances;

        private Vector3 _position;
        private Quaternion _rotation;

        private void OnEnable()
        {
            AddInstance();

            if (m_Instance.valid && m_FollowTransform)
                AddTracking();
        }

        private void OnDisable()
        {
            m_Instance.Remove();
            RemoveTracking();
        }

        /// <summary>
        /// Update NavMeshData instance with the current position and rotation
        /// </summary>
        public void UpdateInstance()
        {
            m_Instance.Remove();
            AddInstance();
        }

        private void AddInstance()
        {
    #if UNITY_EDITOR
            if (m_Instance.valid)
            {
                Debug.LogError("Instance is already added: " + this);
                return;
            }
    #endif
            var t = transform;
            if (m_NavMesh)
            {
                m_Instance = NavMesh.AddNavMeshData(m_NavMesh, t.position, t.rotation);
            }

            _rotation = t.rotation;
            _position = t.position;
        }

        private void AddTracking()
        {
    #if UNITY_EDITOR
            // At runtime we don't want linear lookup
            if (s_TrackedInstances.Contains(this))
            {
                Debug.LogError("Double registration of " + this);
                return;
            }
    #endif
            if (s_TrackedInstances.Count == 0)
            {
                NavMesh.onPreUpdate += UpdateTrackedInstances;
            }
            s_TrackedInstances.Add(this);
        }

        private void RemoveTracking()
        {
            s_TrackedInstances.Remove(this);
            if (s_TrackedInstances.Count == 0)
            {
                NavMesh.onPreUpdate -= UpdateTrackedInstances;
            }
        }

        private void SetFollowTransform(bool value)
        {
            if (m_FollowTransform == value)
            {
                return;
            }
            m_FollowTransform = value;
            if (value)
            {
                AddTracking();
            }
            else
            {
                RemoveTracking();
            }
        }

        private bool HasMoved()
        {
            var t = transform;
            return _position != t.position || _rotation != t.rotation;
        }

        private static void UpdateTrackedInstances()
        {
            foreach (var instance in s_TrackedInstances)
            {
                if (instance.HasMoved())
                {
                    instance.UpdateInstance();
                }
            }
        }

    #if UNITY_EDITOR
        private void OnValidate()
        {
            // Only when the instance is valid (OnEnable is called) - we react to changes caused by serialization
            if (!m_Instance.valid)
            {
                return;
            }
            // OnValidate can be called several times - avoid double registration
            // We afford this linear lookup in the editor only
            if (!m_FollowTransform)
            {
                RemoveTracking();
            }
            else if (!s_TrackedInstances.Contains(this))
            {
                AddTracking();
            }
        }
    #endif
    }
}

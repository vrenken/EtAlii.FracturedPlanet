using UnityEngine;
using UnityEngine.AI;

// ReSharper disable once CheckNamespace
namespace Unity.AI.Navigation.Samples
{
    /// <summary>
    /// Walk to a random position and repeat
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class RandomWalk : MonoBehaviour
    {
        public float m_Range = 25.0f;
        private NavMeshAgent _agent;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (_agent.pathPending || _agent.remainingDistance > 0.1f)
            {
                return;
            }

            _agent.destination = m_Range * Random.insideUnitCircle;
        }
    }
}

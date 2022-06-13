using UnityEngine;
using UnityEngine.AI;

// ReSharper disable once CheckNamespace
namespace Unity.AI.Navigation.Samples
{
    /// <summary>
    /// Use physics raycast hit from mouse click to set agent destination
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class ClickToMove : MonoBehaviour
    {
        private NavMeshAgent m_Agent;
        private RaycastHit _hitInfo;

        private void Start()
        {
            m_Agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray.origin, ray.direction, out _hitInfo))
                {
                    m_Agent.destination = _hitInfo.point;
                }
            }
        }
    }
}

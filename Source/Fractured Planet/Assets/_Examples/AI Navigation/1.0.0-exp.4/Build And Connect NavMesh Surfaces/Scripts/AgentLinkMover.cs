using UnityEngine;
using UnityEngine.AI;
using System.Collections;

// ReSharper disable once CheckNamespace
namespace Unity.AI.Navigation.Samples
{
    public enum OffMeshLinkMoveMethod
    {
        Teleport,
        NormalSpeed,
        Parabola,
        Curve
    }

    /// <summary>
    /// Move an agent when traversing a OffMeshLink given specific animated methods
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentLinkMover : MonoBehaviour
    {
        public OffMeshLinkMoveMethod m_Method = OffMeshLinkMoveMethod.Parabola;
        public AnimationCurve m_Curve = new();

        private IEnumerator Start()
        {
            var agent = GetComponent<NavMeshAgent>();
            agent.autoTraverseOffMeshLink = false;
            while (true)
            {
                if (agent.isOnOffMeshLink)
                {
                    if (m_Method == OffMeshLinkMoveMethod.NormalSpeed)
                    {
                        yield return StartCoroutine(NormalSpeed(agent));
                    }
                    else if (m_Method == OffMeshLinkMoveMethod.Parabola)
                    {
                        yield return StartCoroutine(Parabola(agent, 2.0f, 0.5f));
                    }
                    else if (m_Method == OffMeshLinkMoveMethod.Curve)
                    {
                        yield return StartCoroutine(Curve(agent, 0.5f));
                    }
                    agent.CompleteOffMeshLink();
                }

                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private IEnumerator NormalSpeed(NavMeshAgent agent)
        {
            var data = agent.currentOffMeshLinkData;
            var endPos = data.endPos + Vector3.up * agent.baseOffset;
            while (agent.transform.position != endPos)
            {
                agent.transform.position =
                    Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
        {
            var data = agent.currentOffMeshLinkData;
            var startPos = agent.transform.position;
            var endPos = data.endPos + Vector3.up * agent.baseOffset;
            var normalizedTime = 0.0f;
            while (normalizedTime < 1.0f)
            {
                var yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
                agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
                normalizedTime += Time.deltaTime / duration;
                yield return null;
            }
        }

        private IEnumerator Curve(NavMeshAgent agent, float duration)
        {
            var data = agent.currentOffMeshLinkData;
            var startPos = agent.transform.position;
            var endPos = data.endPos + Vector3.up * agent.baseOffset;
            var normalizedTime = 0.0f;
            while (normalizedTime < 1.0f)
            {
                var yOffset = m_Curve.Evaluate(normalizedTime);
                agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
                normalizedTime += Time.deltaTime / duration;
                yield return null;
            }
        }
    }
}

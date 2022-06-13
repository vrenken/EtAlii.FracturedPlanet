using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Unity.AI.Navigation.Samples
{
    /// <summary>
    /// Enables a behaviour when a rigidbody settles movement
    /// otherwise disables the behaviour
    /// </summary>
    public class EnableIffSleeping : MonoBehaviour
    {
        public Behaviour m_Behaviour;
        private Rigidbody _rigidbody;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_rigidbody == null || m_Behaviour == null)
                return;

            if (_rigidbody.IsSleeping() && !m_Behaviour.enabled)
                m_Behaviour.enabled = true;

            if (!_rigidbody.IsSleeping() && m_Behaviour.enabled)
                m_Behaviour.enabled = false;
        }
    }
}

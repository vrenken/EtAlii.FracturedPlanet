using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Unity.AI.Navigation.Samples
{
    /// <summary>
    /// Makes a transform oscillate relative to its start position
    /// </summary>
    public class Oscillator : MonoBehaviour
    {
        public float m_Amplitude = 1.0f;
        public float m_Period = 1.0f;
        public Vector3 m_Direction = Vector3.up;
        private Vector3 _startPosition;

        private void Start()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            var pos = _startPosition + m_Direction * m_Amplitude * Mathf.Sin(2.0f * Mathf.PI * Time.time / m_Period);
            transform.position = pos;
        }
    }
}

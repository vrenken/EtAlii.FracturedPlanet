using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Unity.AI.Navigation.Samples
{
    /// <summary>
    /// Destroy owning GameObject if any collider with a specific tag is trespassing
    /// </summary>
    public class DestroyOnTrigger : MonoBehaviour
    {
        public string m_Tag = "Player";

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(m_Tag))
                Destroy(gameObject);
        }
    }
}

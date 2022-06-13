using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Unity.AI.Navigation.Samples
{
    /// <summary>
    /// Prefab spawner with a key input
    /// </summary>
    public class SpawnPrefabOnKeyDown : MonoBehaviour
    {
        public GameObject m_Prefab;
        public KeyCode m_KeyCode;

        private void Update()
        {
            var t = transform;
            if (Input.GetKeyDown(m_KeyCode) && m_Prefab != null)
            {
                Instantiate(m_Prefab, t.position, t.rotation);
            }
        }
    }
}

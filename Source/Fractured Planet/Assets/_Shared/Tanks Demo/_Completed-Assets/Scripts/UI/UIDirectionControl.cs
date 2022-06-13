using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Complete
{
    public class UIDirectionControl : MonoBehaviour
    {
        // This class is used to make sure world space UI
        // elements such as the health bar face the correct direction.

        public bool m_UseRelativeRotation = true;       // Use relative rotation should be used for this gameobject?


        private Quaternion _relativeRotation;          // The local rotation at the start of the scene.


        private void Start ()
        {
            _relativeRotation = transform.parent.localRotation;
        }


        private void Update ()
        {
            if (m_UseRelativeRotation)
                transform.rotation = _relativeRotation;
        }
    }
}

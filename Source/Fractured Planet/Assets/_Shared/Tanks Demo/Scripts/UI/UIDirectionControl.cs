using UnityEngine;

// ReSharper disable once CheckNamespace
public class UIDirectionControl : MonoBehaviour
{
    public bool m_UseRelativeRotation = true;


    private Quaternion _relativeRotation;


    private void Start()
    {
        _relativeRotation = transform.parent.localRotation;
    }


    private void Update()
    {
        if (m_UseRelativeRotation)
            transform.rotation = _relativeRotation;
    }
}

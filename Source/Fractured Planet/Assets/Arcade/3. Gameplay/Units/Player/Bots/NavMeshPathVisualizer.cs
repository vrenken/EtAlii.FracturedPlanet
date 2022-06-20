namespace EtAlii.FracturedPlanet
{
    using UnityEngine;
    using UnityEngine.AI;

    [ExecuteInEditMode]
    public class NavMeshPathVisualizer : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private Color _color = Color.white;

        private float _interval = 2;
        private float _nextUpdate;

#if UNITY_EDITOR
        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        public void Update()
        {
            if (!(Time.time >= _nextUpdate))
            {
                return;
            }

            DrawPath();
            _nextUpdate += _interval;
        }

        private void DrawPath()
        {
            var path = _agent.path;
            if (path.corners.Length < 2)
            {
                return;
            }

            _color = path.status switch
            {
                NavMeshPathStatus.PathComplete => Color.white,
                NavMeshPathStatus.PathInvalid => Color.red,
                NavMeshPathStatus.PathPartial => Color.yellow,
                _ => _color
            };

            var previousCorner = path.corners[0];

            var i = 1;
            while (i < path.corners.Length)
            {
                var currentCorner = path.corners[i];
                Debug.DrawLine(previousCorner, currentCorner, _color, 2, false);
                previousCorner = currentCorner;
                i++;
            }
        }
#endif
    }
}

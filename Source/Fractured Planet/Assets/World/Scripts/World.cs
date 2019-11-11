namespace EtAlii.FracturedPlanet.World
{
    using UnityEngine;

    public class World : MonoBehaviour
    {
        public float curveRadius, pipeRadius;

        public int curveSegmentCount, pipeSegmentCount;

        private Mesh _mesh;
        private Vector3[] _vertices;
        private int[] _triangles;


        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Awake()
        {
            GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
            _mesh.name = "Pipe";
            SetVertices();
            SetTriangles();
            _mesh.RecalculateNormals();
        }

        private Vector3 GetPointOnTorus(float u, float v)
        {
            Vector3 p;
            var r = (curveRadius + pipeRadius * Mathf.Cos(v));
            p.z = r * Mathf.Sin(u);
            p.x = r * Mathf.Cos(u);
            p.y = pipeRadius * Mathf.Sin(v);
            return p;
        }

        private void CreateQuadRing(float u, int i)
        {
            var vStep = (2f * Mathf.PI) / pipeSegmentCount;
            var ringOffset = pipeSegmentCount * 4;

            var vertex = GetPointOnTorus(u, 0f);
            for (var v = 1; v <= pipeSegmentCount; v++, i += 4)
            {
                _vertices[i] = _vertices[i - ringOffset + 2];
                _vertices[i + 1] = _vertices[i - ringOffset + 3];
                _vertices[i + 2] = vertex;
                _vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
            }
        }

        private void SetTriangles()
        {
            _triangles = new int[pipeSegmentCount * curveSegmentCount * 6];
            for (int t = 0, i = 0; t < _triangles.Length; t += 6, i += 4)
            {
                _triangles[t] = i;
                _triangles[t + 1] = _triangles[t + 4] = i + 1;
                _triangles[t + 2] = _triangles[t + 3] = i + 2;
                _triangles[t + 5] = i + 3;
            }

            _mesh.triangles = _triangles;
        }

        private void CreateFirstQuadRing(float u)
        {
            var vStep = (2f * Mathf.PI) / pipeSegmentCount;

            var vertexA = GetPointOnTorus(0f, 0f);
            var vertexB = GetPointOnTorus(u, 0f);
            for (var v = 1; v <= pipeSegmentCount; v++)
            {
                vertexA = GetPointOnTorus(0f, v * vStep);
                vertexB = GetPointOnTorus(u, v * vStep);
            }
        }

        private void SetVertices()
        {
            _vertices = new Vector3[pipeSegmentCount * curveSegmentCount * 4];
            var uStep = (2f * Mathf.PI) / curveSegmentCount;
            CreateFirstQuadRing(uStep);
            var iDelta = pipeSegmentCount * 4;
            for (int u = 2, i = iDelta; u <= curveSegmentCount; u++, i += iDelta)
            {
                CreateQuadRing(u * uStep, i);
            }

            _mesh.vertices = _vertices;
        }

        private void OnDrawGizmos()
        {
            var uStep = (2f * Mathf.PI) / curveSegmentCount;
            var vStep = (2f * Mathf.PI) / pipeSegmentCount;

            for (var u = 0; u < curveSegmentCount; u++)
            {
                for (var v = 0; v < pipeSegmentCount; v++)
                {
                    var point = GetPointOnTorus(u * uStep, v * vStep);
                    Gizmos.color = new Color(
                        1f,
                        (float) v / pipeSegmentCount,
                        (float) u / curveSegmentCount);
                    Gizmos.DrawSphere(point, 0.1f);
                }
            }
        }
    }
}
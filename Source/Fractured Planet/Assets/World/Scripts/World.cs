using UnityEngine;

public class World : MonoBehaviour
{
    public float curveRadius, pipeRadius;

    public int curveSegmentCount, pipeSegmentCount;
    
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void Awake () {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Pipe";
        SetVertices();
        SetTriangles();
        mesh.RecalculateNormals();
    }
    
    private Vector3 GetPointOnTorus (float u, float v) {
        Vector3 p;
        var r = (curveRadius + pipeRadius * Mathf.Cos(v));
        p.z = r * Mathf.Sin(u);
        p.x = r * Mathf.Cos(u);
        p.y = pipeRadius * Mathf.Sin(v);
        return p;
    }
    
    private void CreateQuadRing (float u, int i) {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;
        int ringOffset = pipeSegmentCount * 4;
		
        Vector3 vertex = GetPointOnTorus(u, 0f);
        for (int v = 1; v <= pipeSegmentCount; v++, i += 4) {
            vertices[i] = vertices[i - ringOffset + 2];
            vertices[i + 1] = vertices[i - ringOffset + 3];
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
        }
    }
    
    private void SetTriangles () {
        triangles = new int[pipeSegmentCount * curveSegmentCount * 6];
        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4) {
            triangles[t] = i;
            triangles[t + 1] = triangles[t + 4] = i + 1;
            triangles[t + 2] = triangles[t + 3] = i + 2;
            triangles[t + 5] = i + 3;
        }
        mesh.triangles = triangles;
    }
    
    private void CreateFirstQuadRing (float u) {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;

        Vector3 vertexA = GetPointOnTorus(0f, 0f);
        Vector3 vertexB = GetPointOnTorus(u, 0f);
        for (int v = 1; v <= pipeSegmentCount; v++) {
            vertexA = GetPointOnTorus(0f, v * vStep);
            vertexB = GetPointOnTorus(u, v * vStep);
        }
    }
    private void SetVertices () {
        vertices = new Vector3[pipeSegmentCount * curveSegmentCount * 4];
        float uStep = (2f * Mathf.PI) / curveSegmentCount;
        CreateFirstQuadRing(uStep);
        int iDelta = pipeSegmentCount * 4;
        for (int u = 2, i = iDelta; u <= curveSegmentCount; u++, i += iDelta) {
            CreateQuadRing(u * uStep, i);
        }
        mesh.vertices = vertices;
    }
    
    private void OnDrawGizmos () 
    {
        var uStep = (2f * Mathf.PI) / curveSegmentCount;
        var vStep = (2f * Mathf.PI) / pipeSegmentCount;

        for (var u = 0; u < curveSegmentCount; u++) {
            for (var v = 0; v < pipeSegmentCount; v++) {
                var point = GetPointOnTorus(u * uStep, v * vStep);
                Gizmos.color = new Color(
                    1f,
                    (float)v / pipeSegmentCount,
                    (float)u / curveSegmentCount);
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
}

namespace EtAlii.FracturedPlanet.Navigation
{

    using UnityEngine;

    public class Chunk : MonoBehaviour
    {
        [HideInInspector] public bool readyForUpdate;
        [HideInInspector] public Point[,,] points;
        [HideInInspector] public int chunkSize;
        [HideInInspector] public Vector3Int position;

        [HideInInspector] public float isoLevel;
        [HideInInspector] public int seed;

        [HideInInspector] public MarchingCubesMeshBuilder marchingCubesMeshBuilder;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private DensityGenerator _densityGenerator;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
        }

        private void Start()
        {
            Generate();
        }

        private void Update()
        {
            if (!readyForUpdate) return;

            Generate();
            readyForUpdate = false;
        }

        private void Generate()
        {
            var mesh = marchingCubesMeshBuilder.Build(points);
            _meshFilter.sharedMesh = mesh;
            _meshCollider.sharedMesh = mesh;
        }

        public Point GetPoint(int x, int y, int z)
        {
            return points[x, y, z];
        }

        public void SetDensity(float density, int x, int y, int z)
        {
            points[x, y, z].density = density;
        }

        public void SetDensity(float density, Vector3Int pos)
        {
            SetDensity(density, pos.x, pos.y, pos.z);
        }
    }
}
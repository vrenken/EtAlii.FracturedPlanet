namespace EtAlii.FracturedPlanet.Navigation
{

    using UnityEngine;

    public class Chunk : MonoBehaviour
    {
        [HideInInspector] public bool readyForUpdate;
        [HideInInspector] public Voxel[,,] voxels;
        [HideInInspector] public Vector3Int position;

        [HideInInspector] public MarchingCubesMeshBuilder marchingCubesMeshBuilder;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private DensityGenerator _densityGenerator;

//        private Bounds _bounds;

//        private void OnDrawGizmos()
//        {
//            Gizmos.DrawWireCube(_bounds.center, _bounds.size);
//        }
//
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
        }

        private void Start()
        {
//            UpdateBounds();

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
            var mesh = marchingCubesMeshBuilder.Build(voxels);
            _meshFilter.sharedMesh = mesh;
            _meshCollider.sharedMesh = mesh;
        }

        public Voxel GetPoint(int x, int y, int z)
        {
            return voxels[x, y, z];
        }

        public void SetDensity(float density, int x, int y, int z)
        {
            voxels[x, y, z].density = density;
        }

        public void SetDensity(float density, Vector3Int pos)
        {
            SetDensity(density, pos.x, pos.y, pos.z);
        }
        
//        private void UpdateBounds()
//        {
//            var size = new Vector3Int(chunkSize,chunkSize,chunkSize);
//
//            _bounds.center = transform.position;// midPos;
//            _bounds.size = size;
//        }

    }
}
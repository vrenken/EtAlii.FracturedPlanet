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
        private DensityManager _densityManager;

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
            var mesh = marchingCubesMeshBuilder.Build(voxels);
            _meshFilter.sharedMesh = mesh;
            _meshCollider.sharedMesh = mesh;
        }
    }
}
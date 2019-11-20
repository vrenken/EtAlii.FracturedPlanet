namespace EtAlii.FracturedPlanet.Navigation
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Sector : MonoBehaviour
    {
        /// <summary>
        /// The number of voxels that each sector has in the width, depth and height.
        /// </summary>
        public const int TerrainVoxels = 128;

        /// <summary>
        /// The size (in meters) a sector needs to be in the width, depth and height.
        /// </summary>
        public const float SectorSize = 128;

        /// <summary>
        /// The half size (in meters) a sector needs to be in the width, depth and height.
        /// </summary>
        public const float SectorSizeHalf = SectorSize / 2f;

        /// <summary>
        /// The size of one single voxel (both width, height and depth). 
        /// </summary>
        public const float VoxelSize = SectorSize / SectorSize;

        public SectorInfo info;
        public Terrain terrain;

        public int chunkSize = 8;

        public int sectorWidth = 5;
        public int sectorHeight = 5;
        public int sectorDepth = 5;

        public float isolevel;

        public int seed;

        public GameObject chunkPrefab;

        public Dictionary<Vector3Int, Chunk> chunks;

        private Bounds sectorBounds;

        private DensityGenerator _densityGenerator;
        private ChunkBuilder _chunkBuilder;
        private SectorBuilder _sectorBuilder;

        private void Awake()
        {
            _densityGenerator = new DensityGenerator(seed);
            _chunkBuilder = new ChunkBuilder(_densityGenerator);
            _sectorBuilder = new SectorBuilder(_chunkBuilder);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(sectorBounds.center, sectorBounds.size);
        }

        private void Start()
        {
            sectorBounds = new Bounds();
            UpdateBounds();

            _sectorBuilder.Build(this);
        }


        private Chunk GetChunk(Vector3Int pos)
        {
            return GetChunk(pos.x, pos.y, pos.z);
        }

        public Chunk GetChunk(int x, int y, int z)
        {
            var newX = Math3d.FloorToNearestX(x, chunkSize);
            var newY = Math3d.FloorToNearestX(y, chunkSize);
            var newZ = Math3d.FloorToNearestX(z, chunkSize);

            return chunks[new Vector3Int(newX, newY, newZ)];
        }

        public float GetDensity(int x, int y, int z)
        {
            var p = GetPoint(x, y, z);

            return p.density;
        }

        public float GetDensity(Vector3Int pos)
        {
            return GetDensity(pos.x, pos.y, pos.z);
        }

        public Voxel GetPoint(int x, int y, int z)
        {
            var chunk = GetChunk(x, y, z);

            var p = chunk.GetPoint(
                Math3d.Mod(x, chunkSize),
                Math3d.Mod(y, chunkSize),
                Math3d.Mod(z, chunkSize));

            return p;
        }

        public void SetDensity(float density, int sectorPosX, int sectorPosY, int sectorPosZ, bool setReadyForUpdate,
            Chunk[] initChunks)
        {
            var dp = new Vector3Int(sectorPosX, sectorPosY, sectorPosZ);

            var lastChunkPos = Math3d.FloorToNearestX(dp, chunkSize);

            for (var i = 0; i < 8; i++)
            {
                var chunkPos = Math3d.FloorToNearestX(dp - LookupTables.CubePoints[i], chunkSize);

                if (i != 0 && chunkPos == lastChunkPos)
                {
                    continue;
                }

                var chunk = GetChunk(chunkPos);

                lastChunkPos = chunk.position;

                var localPos = Math3d.Mod(dp - chunk.position, chunkSize + 1);

                chunk.SetDensity(density, localPos);
                if (setReadyForUpdate)
                    chunk.readyForUpdate = true;
            }
        }

        public void SetDensity(float density, Vector3Int pos, bool setReadyForUpdate, Chunk[] initChunks)
        {
            SetDensity(density, pos.x, pos.y, pos.z, setReadyForUpdate, initChunks);
        }

        private void UpdateBounds()
        {
            var middleX = sectorWidth * chunkSize / 2f;
            var middleY = sectorHeight * chunkSize / 2f;
            var middleZ = sectorDepth * chunkSize / 2f;

            var midPos = new Vector3(middleX, middleY, middleZ);

            var size = new Vector3Int(
                sectorWidth * chunkSize,
                sectorHeight * chunkSize,
                sectorDepth * chunkSize);

            sectorBounds.center = midPos;
            sectorBounds.size = size;
        }

        public bool IsPointInsideSector(int x, int y, int z)
        {
            return IsPointInsideSector(new Vector3Int(x, y, z));
        }

        public bool IsPointInsideSector(Vector3Int point)
        {
            return sectorBounds.Contains(point);
        }

    }
}
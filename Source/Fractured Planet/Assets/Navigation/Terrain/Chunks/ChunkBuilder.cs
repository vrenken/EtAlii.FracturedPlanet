namespace EtAlii.FracturedPlanet.Navigation
{
    using UnityEngine;

    public class ChunkBuilder
    {
        private readonly DensityManager _densityManager;

        public ChunkBuilder(DensityManager densityManager)
        {
            _densityManager = densityManager;
        }

        public Chunk Build(Sector sector, int chunkX, int chunkY, int chunkZ, out Vector3Int position)
        {
            var chunkSize = sector.chunkSize;
            position = new Vector3Int(chunkX, chunkY, chunkZ);

            var gameObject = Object.Instantiate(sector.chunkPrefab, position, Quaternion.identity);
            gameObject.name = $"Chunk [{chunkX}, {chunkY}, {chunkZ}]";
            gameObject.transform.parent = sector.transform;

            var chunk = gameObject.GetComponent<Chunk>();

            chunk.position = position;

            var sectorPosX = position.x;
            var sectorPosY = position.y;
            var sectorPosZ = position.z;

            chunk.voxels = new Voxel[chunkSize + 1, chunkSize + 1, chunkSize + 1];
            chunk.marchingCubesMeshBuilder = new MarchingCubesMeshBuilder(sector.isoLevel);

            for (var x = 0; x < chunk.voxels.GetLength(0); x++)
            {
                for (var y = 0; y < chunk.voxels.GetLength(1); y++)
                {
                    for (var z = 0; z < chunk.voxels.GetLength(2); z++)
                    {
                        var voxelPosition = new Vector3Int(x, y, z);
                        var density = _densityManager.CalculateDensity(x + sectorPosX, y + sectorPosY, z + sectorPosZ);
                        chunk.voxels[x, y, z] = new Voxel(voxelPosition, density);
                    }
                }
            }

            return chunk;
        }
    }
}

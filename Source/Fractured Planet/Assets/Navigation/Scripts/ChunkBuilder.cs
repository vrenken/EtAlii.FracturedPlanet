namespace EtAlii.FracturedPlanet.Navigation
{
    using UnityEngine;

    public class ChunkBuilder
    {
        private readonly DensityGenerator _densityGenerator;

        public ChunkBuilder(DensityGenerator densityGenerator)
        {
            _densityGenerator = densityGenerator;
        }

        public void Initialize(Chunk chunk, Sector sector, int chunkSize, Vector3Int position)
        {
            chunk.chunkSize = chunkSize;
            chunk.position = position;
            chunk.isoLevel = sector.isolevel;

            var sectorPosX = position.x;
            var sectorPosY = position.y;
            var sectorPosZ = position.z;

            chunk.voxels = new Voxel[chunkSize + 1, chunkSize + 1, chunkSize + 1];

            chunk.seed = sector.seed;
            chunk.marchingCubesMeshBuilder = new MarchingCubesMeshBuilder(chunk.voxels, chunk.isoLevel, chunk.seed);

            for (var x = 0; x < chunk.voxels.GetLength(0); x++)
            {
                for (var y = 0; y < chunk.voxels.GetLength(1); y++)
                {
                    for (var z = 0; z < chunk.voxels.GetLength(2); z++)
                    {
                        var voxelPosition = new Vector3Int(x, y, z);
                        var density = _densityGenerator.CalculateDensity(x + sectorPosX, y + sectorPosY, z + sectorPosZ); 
                        chunk.voxels[x, y, z] = new Voxel(voxelPosition, density);
                    }
                }
            }
        }
    }
}
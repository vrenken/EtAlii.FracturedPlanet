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

            chunk.points = new Point[chunkSize + 1, chunkSize + 1, chunkSize + 1];

            chunk.seed = sector.seed;
            chunk.marchingCubesMeshBuilder = new MarchingCubesMeshBuilder(chunk.points, chunk.isoLevel, chunk.seed);

            for (var x = 0; x < chunk.points.GetLength(0); x++)
            {
                for (var y = 0; y < chunk.points.GetLength(1); y++)
                {
                    for (var z = 0; z < chunk.points.GetLength(2); z++)
                    {
                        chunk.points[x, y, z] = new Point(
                            new Vector3Int(x, y, z),
                            _densityGenerator.CalculateDensity(x + sectorPosX, y + sectorPosY, z + sectorPosZ)
                        );
                    }
                }
            }
        }
    }
}
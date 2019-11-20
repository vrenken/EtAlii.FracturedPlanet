namespace EtAlii.FracturedPlanet.Navigation
{
    using System.Collections.Generic;
    using UnityEngine;

    public class SectorBuilder
    {
        private readonly ChunkBuilder _chunkBuilder;

        public SectorBuilder(ChunkBuilder chunkBuilder)
        {
            _chunkBuilder = chunkBuilder;
        }

        public void Build(Sector sector)
        {
            sector.chunks = new Dictionary<Vector3Int, Chunk>(sector.sectorWidth * sector.sectorHeight * sector.sectorDepth);

            for (var x = 0; x < sector.sectorWidth; x++)
            {
                for (var y = 0; y < sector.sectorHeight; y++)
                {
                    for (var z = 0; z < sector.sectorDepth; z++)
                    {
                        var chunk = _chunkBuilder.Build(sector, x * sector.chunkSize, y * sector.chunkSize, z * sector.chunkSize, out var position);
                        sector.chunks.Add(position, chunk);
                    }
                }
            }
        }
    }
}
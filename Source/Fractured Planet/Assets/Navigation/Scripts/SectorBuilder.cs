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
            sector.chunks =
                new Dictionary<Vector3Int, Chunk>(sector.sectorWidth * sector.sectorHeight * sector.sectorDepth);
            ;

            for (var x = 0; x < sector.sectorWidth; x++)
            {
                for (var y = 0; y < sector.sectorHeight; y++)
                {
                    for (var z = 0; z < sector.sectorDepth; z++)
                    {
                        CreateChunk(sector, x * sector.chunkSize, y * sector.chunkSize, z * sector.chunkSize);
                    }
                }
            }
        }

        private void CreateChunk(Sector sector, int x, int y, int z)
        {
            var position = new Vector3Int(x, y, z);

            var gameObject = Object.Instantiate(sector.chunkPrefab, position, Quaternion.identity);
            gameObject.name = $"Chunk [{x}, {y}, {z}]";
            gameObject.transform.parent = sector.transform;

            var chunk = gameObject.GetComponent<Chunk>();

            _chunkBuilder.Initialize(chunk, sector, sector.chunkSize, position);
            sector.chunks.Add(position, chunk);
        }
    }
}
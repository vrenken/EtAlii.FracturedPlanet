namespace EtAlii.FracturedPlanet.Navigation
{
    using UnityEngine;

    public class DensityManager
    {
        public FastNoise Noise { get; }

        public DensityManager(int seed)
        {
            Noise = new FastNoise(seed);
        }

        public float CalculateDensity(int sectorPosX, int sectorPosY, int sectorPosZ)
        {
            var result = SectorDensity(sectorPosX, sectorPosY, sectorPosZ, .1f);
            return Math3d.Clamp01(result);
        }

        public float SphereDensity(int sectorPosX, int sectorPosY, int sectorPosZ, int radius)
        {
            return sectorPosX * sectorPosX + sectorPosY * sectorPosY + sectorPosZ * sectorPosZ - radius * radius;
        }

        public float SectorDensity(int sectorPosX, int sectorPosY, int sectorPosZ, float noiseScale)
        {
            var noise = Noise.GetPerlin(sectorPosX / noiseScale, sectorPosZ / noiseScale);
            return sectorPosY - Math3d.Map(noise, -1, 1, 0, 1) * 10 - 10;
        }

        public void SetDensity(Chunk chunk, float density, int x, int y, int z)
        {
            chunk.voxels[x, y, z].Density = density;
        }

        public void SetDensity(Chunk chunk, float density, Vector3Int pos)
        {
            SetDensity(chunk, density, pos.x, pos.y, pos.z);
        }

        public float FlatPlane(int y, float height)
        {
            return y - height + 0.5f;
        }

        public float Union(float d1, float d2)
        {
            return Math3d.Min(d1, d2);
        }

        public float Subtract(float d1, float d2)
        {
            return Math3d.Max(-d1, d2);
        }

        public float Intersection(float d1, float d2)
        {
            return Math3d.Max(d1, d2);
        }
    }
}
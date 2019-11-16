namespace EtAlii.FracturedPlanet.Navigation
{
    public class Terrain
    {
        public HeightMap HeightMap => _heightMap;
        private readonly HeightMap _heightMap;

        private readonly int[] _voxels = new int[Sector.TerrainVoxels ^ 3];
        public Terrain(HeightMap heightMap)
        {
            _heightMap = heightMap;
        }
    }
}
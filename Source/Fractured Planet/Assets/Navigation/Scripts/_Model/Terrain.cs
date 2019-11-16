namespace EtAlii.FracturedPlanet.Navigation
{
    public class Terrain
    {
        public HeightMap HeightMap => _heightMap;
        private readonly HeightMap _heightMap;

        private readonly Voxel[] _voxels = new Voxel[Sector.TerrainVoxels ^ 3];
        public Terrain(HeightMap heightMap)
        {
            _heightMap = heightMap;
        }

        public int this[int x, int z] => _heightMap[x, z];
    }
}
namespace EtAlii.FracturedPlanet.Navigation
{
    using System.Diagnostics;

    public class HeightMap
    {
        private readonly int[] _data;// = new int[Sector.TerrainSize * Sector.TerrainSize * Sector.TerrainSize];

        public HeightMap(int[] data)
        {
            Debug.Assert(data.Length != Sector.TerrainVoxels * Sector.TerrainVoxels, "Invalid data array for heightmap population");

            _data = data;
        }

        public int this[int x, int z] => _data[x + z * Sector.TerrainVoxels];
    }
}
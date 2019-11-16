namespace EtAlii.FracturedPlanet.Navigation
{
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
    }
}
namespace EtAlii.FracturedPlanet.Navigation
{
    using UnityEngine;

    public class TerrainMeshBuilder
    {
        private readonly Vector3 _voxelScale = new Vector3(Sector.VoxelSize, Sector.VoxelSize, Sector.VoxelSize);

        public void Build(Sector sector, GameObject voxelPrefab)
        {
            var startPosition = (sector.transform.position + Vector3.one) * -Sector.SectorSizeHalf;
            
            for (var z = 0; z < Sector.TerrainVoxels; z++)
            {
                for (var x = 0; x < Sector.TerrainVoxels; x++)
                {
                    var position = startPosition + new Vector3(x * Sector.VoxelSize, 0, z * Sector.VoxelSize);

                    var height = sector.terrain.HeightMap[x, z];
                    position.y = height;// * 0.1f;

                    var voxelGameObject = Object.Instantiate(voxelPrefab, sector.transform);
                    var voxelTransform = voxelGameObject.transform;
                    voxelGameObject.name = $"Voxel [{x}, {z}]";
                    voxelTransform.position = position;
                    voxelTransform.localScale = _voxelScale;
                    //var voxel = sectorGameObject.GetComponent<Sector>();
                }
            }
        }
    }
}
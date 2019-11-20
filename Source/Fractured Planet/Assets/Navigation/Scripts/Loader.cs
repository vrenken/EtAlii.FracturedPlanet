namespace EtAlii.FracturedPlanet.Navigation
{
    using System;
    using UnityEngine;

    public class Loader : MonoBehaviour
    {
        public GameObject terrain;

        public GameObject voxelPrefab;
        public GameObject sectorPrefab;
        
        // Start is called before the first frame update
        void Start()
        {
            // Conduct scene loading.

            var x = 0;
            var y = 0;
            var sectorName = $"Sector [{x}-{y}]";

            var sectorInfo = new SectorInfo { Id = Guid.NewGuid(), Name = sectorName, X = x, Y = y };
            var heightMap = new HeightMapBuilder().Build();
            
            var sectorGameObject = Instantiate(sectorPrefab, terrain.transform);
            var sector = sectorGameObject.GetComponent<Sector>();
            sectorGameObject.name = sectorInfo.Name;
            sector.info = sectorInfo;
            
            sector.terrain = new Terrain(heightMap);

            new TerrainMeshBuilder().Build(sector, voxelPrefab);
            
            // No need to keep the loader intact. It's a Loader...
            Destroy(gameObject); 
        }
    }
}
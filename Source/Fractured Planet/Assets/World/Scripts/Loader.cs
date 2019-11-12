namespace EtAlii.FracturedPlanet.World
{
    using System;
    using EtAlii.FracturedPlanet.Sector;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class Loader : MonoBehaviour
    {
        private const int SectorsToAdd = 200;

        private const int MaxU = 50;
        private const int MaxV = 50;
        
        // Start is called before the first frame update
        void Start()
        {
            
            for (var i = 0; i < SectorsToAdd; i++)
            {
                var u = Random.Range(-MaxU,+MaxU);
                var v = Random.Range(-MaxV, MaxV);
                var id = Guid.NewGuid();

                var sector = new Sector {U = u, V = v, Id = id, Name = $"Sector {id}"};
                SectorManager.Instance.Add(sector);
            }
            
            // No need to keep the loader intact. It's a Loader...
            Destroy(gameObject); 
        }
    }
}
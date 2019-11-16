namespace EtAlii.FracturedPlanet.World
{
    using System;
    using EtAlii.FracturedPlanet.Navigation;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class Loader : MonoBehaviour
    {
        private const int SectorsToAdd = (int)(MaxX * MaxY * 0.8f);

        private const int MaxX = 50;
        private const int MaxY = 20;

        public Galaxy galaxy;
        public MainMenu menu;
        
        
        // Start is called before the first frame update
        private void Start()
        {
            
            for (var i = 0; i < SectorsToAdd; i++)
            {
                int x, y;
                do
                {
                    x = Random.Range(0, MaxX);
                    y = Random.Range(0, MaxY);
                } while (SectorManager.Instance.IsPopulated(x, y)); // We need to find an empty place on the toroid.
             
                var id = Guid.NewGuid();
                var sectorName = $"Sector [{x}-{y}]";

                var sectorInfo = new SectorInfo {X = x, Y = y, Id = id, Name = sectorName};
                SectorManager.Instance.Add(sectorInfo);
            }

            menu.Activate();
            //galaxy.Activate();

            // No need to keep the loader intact. It's a Loader...
            Destroy(gameObject); 
        }
    }
}
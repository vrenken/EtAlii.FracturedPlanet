namespace EtAlii.FracturedPlanet.World
{
    using System.Collections.Specialized;
    using System.Linq;
    using EtAlii.FracturedPlanet.Sector;
    using UnityEngine;

    public class WorldTorus : MonoBehaviour
    {
        public GameObject sectorTilePrefab;

        /// <summary>
        /// The radius of the torus itself, i.e. the donut radius. 
        /// </summary>
        public float majorRadius;

        /// <summary>
        /// The radius of the torus tube, i.e. the thickness of the donut.
        /// </summary>
        public float minorRadius;

        public int majorSegmentCount;
        public int minorSegmentCount;


        // Start is called before the first frame update
        void Start()
        {
            SectorManager.Instance.Changed += OnSectorsChanged;
        }
        
        void OnDestroy()
        {
            SectorManager.Instance.Changed -= OnSectorsChanged;
        }

        private void OnSectorsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Sector sector in e.NewItems)
                    {
                        AddSector(sector);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Sector sector in e.OldItems)
                    {
                        RemoveSector(sector);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    RemoveAllSectors();
                    foreach (Sector sector in SectorManager.Instance.Sectors)
                    {
                        AddSector(sector);
                    }
                    break;
            }
            UpdateSectorPositionsOnWorld();
        }

        private void RemoveAllSectors()
        {
            var sectorTiles = gameObject.GetComponentsInChildren<SectorTile>().ToArray();
            foreach(var sectorTile in sectorTiles)
            {
                Destroy(sectorTile.gameObject);
            }
        }

        private void RemoveSector(Sector sector)
        {
            var sectorTile = gameObject.GetComponentsInChildren<SectorTile>().SingleOrDefault(st => st.sector == sector);
            if (sectorTile != null)
            {
                Destroy(sectorTile.gameObject);
            }
        }

        private void AddSector(Sector sector)
        {
            var sectorTileGameObject = Instantiate(sectorTilePrefab, transform, true);
            var sectorTile = sectorTileGameObject.GetComponent<SectorTile>();
            sectorTileGameObject.name = sector.Name;
            sectorTile.sector = sector;
        }

        private void UpdateSectorPositionsOnWorld()
        {
            var sectorTiles = gameObject.GetComponentsInChildren<SectorTile>().ToArray();
            
            majorSegmentCount = sectorTiles.Max(tile => tile.sector.X);
            minorSegmentCount = sectorTiles.Max(tile => tile.sector.Y);

            foreach(var sectorTile in sectorTiles)
            {
                sectorTile.UpdatePositionOnWorld(majorRadius, minorRadius, majorSegmentCount, minorSegmentCount);
            }
        }
    }
}
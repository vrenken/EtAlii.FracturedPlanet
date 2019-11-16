namespace EtAlii.FracturedPlanet.World
{
    using System;
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

        private float _verticalRotation = 0;
        private float _horizontalRotation = 0;

        private float _rotationSpeed = 40f;

        public bool autoRotate = false;
        
        private SectorTile[] _sectorTiles;
        
        // Start is called before the first frame update
        void Start()
        {
            SectorManager.Instance.Changed += OnSectorsChanged;
        }
        
        void OnDestroy()
        {
            SectorManager.Instance.Changed -= OnSectorsChanged;
        }

        public void Update()
        {
            // Get the horizontal and vertical axis.
            // By default they are mapped to the arrow keys.
            // The value is in the range -1 to 1
            var verticalRotation = !autoRotate ? Input.GetAxis("Vertical") * _rotationSpeed : 0.1f * _rotationSpeed;
            var horizontalRotation = !autoRotate ? Input.GetAxis("Horizontal") * _rotationSpeed : 0.1f * _rotationSpeed;

            // Make it move 10 meters per second instead of 10 meters per frame...
            verticalRotation *= Time.deltaTime;
            horizontalRotation *= Time.deltaTime;

            if (!(Math.Abs(verticalRotation) > 0.001f) && !(Math.Abs(horizontalRotation) > 0.001f)) return;
            
            _verticalRotation += verticalRotation;
            _horizontalRotation += horizontalRotation;

            UpdateSectorPositionsOnWorld();
        }

        private void OnSectorsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (SectorInfo sector in e.NewItems)
                    {
                        AddSector(sector);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (SectorInfo sector in e.OldItems)
                    {
                        RemoveSector(sector);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    RemoveAllSectors();
                    foreach (SectorInfo sector in SectorManager.Instance.Sectors)
                    {
                        AddSector(sector);
                    }
                    break;
            }
            UpdateSectorPositionsOnWorld();
        }

        private void RemoveAllSectors()
        {
            foreach(var sectorTile in _sectorTiles)
            {
                Destroy(sectorTile.gameObject);
            }
            _sectorTiles = gameObject.GetComponentsInChildren<SectorTile>().ToArray();
        }

        private void RemoveSector(SectorInfo sector)
        {
            var sectorTile = _sectorTiles.SingleOrDefault(st => st.sector == sector);
            if (sectorTile != null)
            {
                Destroy(sectorTile.gameObject);
            }
            _sectorTiles = gameObject.GetComponentsInChildren<SectorTile>().ToArray();
        }

        private void AddSector(SectorInfo sector)
        {
            var sectorTileGameObject = Instantiate(sectorTilePrefab, transform, true);
            var sectorTile = sectorTileGameObject.GetComponent<SectorTile>();
            sectorTileGameObject.name = sector.Name;
            sectorTile.sector = sector;
            _sectorTiles = gameObject.GetComponentsInChildren<SectorTile>().ToArray();
        }

        private void UpdateSectorPositionsOnWorld()
        {
            majorSegmentCount = _sectorTiles.Max(tile => tile.sector.X);
            minorSegmentCount = _sectorTiles.Max(tile => tile.sector.Y);

            foreach(var sectorTile in _sectorTiles)
            {
                sectorTile.UpdatePositionOnWorld(majorRadius, minorRadius, majorSegmentCount, minorSegmentCount, _horizontalRotation, _verticalRotation);
            }
        }
    }
}
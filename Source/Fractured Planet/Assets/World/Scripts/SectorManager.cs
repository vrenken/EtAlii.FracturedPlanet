namespace EtAlii.FracturedPlanet.World
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using EtAlii.FracturedPlanet.Sector;

    public class SectorManager
    {
        public IEnumerable Sectors => _sectors;
        private readonly ObservableCollection<Sector> _sectors;
        
        public static SectorManager Instance { get; } = new SectorManager();

        public event NotifyCollectionChangedEventHandler Changed
        {
            add => _sectors.CollectionChanged += value;
            remove => _sectors.CollectionChanged -= value;
        }
        
        private SectorManager()
        {
            _sectors = new ObservableCollection<Sector>();
        }

        public bool IsPopulated(int x, int y)
        {
            return _sectors.Any(s => s.X == x && s.Y == y);
        }
        public void Add(Sector sector)
        {
            if (_sectors.Any(s => (s.X == sector.X && s.Y == sector.Y) || s.Id == sector.Id))
            {
                throw new InvalidOperationException($"Unable to add sector {sector.Name} - [{sector.Id}].");
            }

            _sectors.Add(sector);
        }
    }
}
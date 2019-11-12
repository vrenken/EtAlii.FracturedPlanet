namespace EtAlii.FracturedPlanet.World
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using EtAlii.FracturedPlanet.Sector;

    public class SectorManager
    {
        public ReadOnlyObservableCollection<Sector> Sectors { get; }
        private readonly ObservableCollection<Sector> _sectors;
        
        public static SectorManager Instance { get; } = new SectorManager();

        public SectorManager()
        {
            _sectors = new ObservableCollection<Sector>();
            Sectors = new ReadOnlyObservableCollection<Sector>(_sectors);
        }
        public void Add(Sector sector)
        {
            if (_sectors.All(s => s.U != sector.U && s.V != sector.V && s.Id != sector.Id))
            {
                _sectors.Add(sector);
            }
        }
    }
}
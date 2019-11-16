namespace EtAlii.FracturedPlanet.Sector
{
    using UnityEngine;

    public class Loader : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            // Conduct scene loading.
            
            // No need to keep the loader intact. It's a Loader...
            Destroy(gameObject);
        }
    }
}

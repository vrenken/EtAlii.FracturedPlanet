namespace EtAlii.FracturedPlanet.World
{
    using UnityEngine;

    public class MainMenu : MonoBehaviour
    {
        public Camera menuCamera;
        public Camera galaxyCamera;

        public Galaxy galaxy;

        public WorldTorus world;
        
        private bool _firstAnimation = true;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Activate()
        {
            var movementDuration = CameraTweener.defaultMovementDuration;
            if (_firstAnimation)
            {
                _firstAnimation = false;
                movementDuration = 5;
            }

            var galaxyCameraTransform = galaxyCamera.transform;

            gameObject.SetActive(true); 
            CameraTweener.Tween(
                galaxyCamera, 
                galaxyCameraTransform,
                menuCamera.transform,
                enumerator => StartCoroutine(enumerator),
                movementDuration,
                () => { world.autoRotate = true; },
                () =>
                {
                    galaxyCamera.enabled = false;
                    menuCamera.enabled = true;
                    galaxyCamera.transform.SetPositionAndRotation(galaxy.startPosition, galaxy.startRotation);
                });
        }
    }
}
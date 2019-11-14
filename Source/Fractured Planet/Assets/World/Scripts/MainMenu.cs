namespace EtAlii.FracturedPlanet.World
{
    using UnityEngine;

    public class MainMenu : MonoBehaviour
    {
        public Camera menuCamera;
        public Camera galaxyCamera;

        public Galaxy galaxy;
        
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
            var galaxyCameraTransform = galaxyCamera.transform;
            var startPosition = galaxyCameraTransform.position; 
            var startRotation = galaxyCameraTransform.rotation; 

            gameObject.SetActive(true); 
            CameraTweener.Tween(
                galaxyCamera, 
                galaxyCameraTransform,
                menuCamera.transform,
                enumerator => StartCoroutine(enumerator),
                () => { },
                () =>
                {
                    galaxyCamera.enabled = false;
                    menuCamera.enabled = true;
                    galaxyCamera.transform.SetPositionAndRotation(startPosition, startRotation);
                });
        }
    }
}
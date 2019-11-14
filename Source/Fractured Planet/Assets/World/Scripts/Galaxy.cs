namespace EtAlii.FracturedPlanet.World
{
    using UnityEngine;

    public class Galaxy : MonoBehaviour
    {
        public Camera menuCamera;
        public Camera galaxyCamera;

        public MainMenu mainMenu;

        public WorldTorus world;
        public Vector3 startPosition;
        public Quaternion startRotation;

        // Start is called before the first frame update
        void Start()
        {
            var galaxyCameraTransform = galaxyCamera.transform;
            startPosition = galaxyCameraTransform.position; 
            startRotation = galaxyCameraTransform.rotation;
            
            // Some stuff to make the intro a bit more interesting.
            galaxyCameraTransform.position = new Vector3(startPosition.x, startPosition.y, -150f);
            galaxyCameraTransform.rotation = startRotation * Quaternion.Euler(0f, -90f, 0f);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                mainMenu.Activate();
                //Application.Quit();
//#if UNITY_EDITOR
//                UnityEditor.EditorApplication.isPlaying = false; 
//#endif
            }

        }

        public void Activate()
        {
            CameraTweener.Tween(
                galaxyCamera,
                menuCamera.transform,
                galaxyCamera.transform,
                enumerator => StartCoroutine(enumerator),
                () =>
                {
                    galaxyCamera.enabled = true;
                    menuCamera.enabled = false;
                },
                () =>
                {
                    world.autoRotate = false;
                    mainMenu.gameObject.SetActive(false);
                    galaxyCamera.transform.SetPositionAndRotation(startPosition, startRotation);
                });
        }
    }
}
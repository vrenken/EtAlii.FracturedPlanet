namespace EtAlii.FracturedPlanet.World
{
    using UnityEngine;

    public class Galaxy : MonoBehaviour
    {
        public Camera menuCamera;
        public Camera galaxyCamera;

        public MainMenu mainMenu;

        // Start is called before the first frame update
        void Start()
        {

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
            var galaxyCameraTransform = galaxyCamera.transform;
            var startPosition = galaxyCameraTransform.position; 
            var startRotation = galaxyCameraTransform.rotation; 

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
                    mainMenu.gameObject.SetActive(false);
                    galaxyCamera.transform.SetPositionAndRotation(startPosition, startRotation);
                });
        }
    }
}
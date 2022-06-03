namespace EtAlii.FracturedPlanet.World
{
    using UnityEngine;

    public class Galaxy : MonoBehaviour
    {
        public Camera menuCamera;
        public Camera galaxyCamera;

        public MainMenu mainMenu;

        public WorldTorus world;

        // Update is called once per frame
        public void Update()
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
                galaxyCameraTransform,
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

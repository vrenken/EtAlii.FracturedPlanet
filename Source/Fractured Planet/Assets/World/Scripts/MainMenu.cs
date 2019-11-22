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

        public void Activate()
        {
            var galaxyCameraTransform = galaxyCamera.transform;

            var startPosition = galaxyCameraTransform.position; 
            var startRotation = galaxyCameraTransform.rotation;
            
            // Some stuff to make the intro a bit more interesting.
            var introStartPosition = new Vector3(startPosition.x, startPosition.y, -150f);
            var introStartRotation = startRotation * Quaternion.Euler(0f, -90f, 0f);

            galaxyCameraTransform.position = _firstAnimation ? introStartPosition : startPosition;
            galaxyCameraTransform.rotation = _firstAnimation ? introStartRotation : startRotation;

            var movementDuration = CameraTweener.DefaultMovementDuration;

            var isFirstAnimation = _firstAnimation;

            if (isFirstAnimation)
            {
                movementDuration = 5;
            }

            CameraTweener.Tween(
                galaxyCamera, 
                galaxyCameraTransform,
                menuCamera.transform,
                enumerator => galaxy.StartCoroutine(enumerator),
                movementDuration,
                () =>
                {
                    world.autoRotate = true;
                    if (isFirstAnimation)
                    {
                        gameObject.SetActive(false);
                    }
                },
                () =>
                {
                    gameObject.SetActive(true);
                    galaxyCamera.enabled = false;
                    menuCamera.enabled = true;
                    galaxyCamera.transform.SetPositionAndRotation(startPosition, startRotation);
                });
            
            _firstAnimation = false;
        }
    }
}
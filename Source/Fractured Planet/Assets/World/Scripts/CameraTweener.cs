namespace EtAlii.FracturedPlanet.World
{
    using System;
    using System.Collections;
    using UnityEngine;

    public static class CameraTweener
    {
        public static float moveDuration = 0.5f;
        public static void Tween(
            Camera camera, 
            Transform sourceTransform, 
            Transform targetTransform, 
            Action<IEnumerator> startCoroutine, 
            Action onBefore = null, 
            Action onAfter = null)
        {
            var targetPosition = targetTransform.position;
            var targetRotation = targetTransform.rotation;

            var sourcePosition = sourceTransform.position;
            var sourceRotation = sourceTransform.rotation;

            var cameraTransform = camera.transform; 
            cameraTransform.position = sourcePosition;
            cameraTransform.rotation = sourceRotation;
            
            startCoroutine(MoveTo(cameraTransform, sourcePosition, sourceRotation, targetPosition, targetRotation, onBefore, onAfter));
        }

        private static IEnumerator MoveTo(Transform cameraTransform, Vector3 sourcePosition, Quaternion sourceRotation, Vector3 targetPosition, Quaternion targetRotation, Action onBefore, Action onAfter)
        {
            cameraTransform.SetPositionAndRotation(sourcePosition, sourceRotation);
            yield return null;
            
            onBefore?.Invoke();
            yield return null;

            var time = 0.0f;
            while (time < moveDuration)
            {
                time += Time.deltaTime;
                cameraTransform.position = Vector3.Lerp (sourcePosition, targetPosition, time/moveDuration);
                cameraTransform.rotation = Quaternion.Lerp(sourceRotation, targetRotation, time / moveDuration);
                yield return null;
            }

            yield return null;
            onAfter?.Invoke();
        }
    }
}
namespace EtAlii.FracturedPlanet.World
{
    using System;
    using System.Collections;
    using UnityEngine;

    public static class CameraTweener
    {
        public static float DefaultMovementDuration = 0.5f;

        public static void Tween(
            Camera camera,
            Transform sourceTransform,
            Transform targetTransform,
            Action<IEnumerator> startCoroutine,
            Action onBefore = null,
            Action onAfter = null)
        {
            Tween(
                camera,
                sourceTransform,
                targetTransform,
                startCoroutine,
                DefaultMovementDuration,
                onBefore,
                onAfter);
        }

        public static void Tween(
            Camera camera, 
            Transform sourceTransform, 
            Transform targetTransform, 
            Action<IEnumerator> startCoroutine,
            float moveDuration, 
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
            
            startCoroutine(MoveTo(cameraTransform, sourcePosition, sourceRotation, targetPosition, targetRotation, onBefore, onAfter, moveDuration));
        }

        private static IEnumerator MoveTo(
            Transform cameraTransform, 
            Vector3 sourcePosition, Quaternion sourceRotation, 
            Vector3 targetPosition, Quaternion targetRotation, 
            Action onBefore, Action onAfter,
            float moveDuration)
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
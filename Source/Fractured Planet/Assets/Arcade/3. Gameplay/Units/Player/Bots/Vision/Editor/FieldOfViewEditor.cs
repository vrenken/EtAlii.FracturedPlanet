// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

namespace EtAlii.FracturedPlanet
{

    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : Editor
    {
        private void OnSceneGUI()
        {
            var fov = (FieldOfView)target;
            var transformPosition = fov.transform.position;
            Handles.color = Color.white;
            Handles.DrawWireArc(transformPosition, Vector3.up, Vector3.forward, 360, fov.radius);

            var yRotation = fov.transform.eulerAngles.y;
            var viewAngle01 = DirectionFromAngle(yRotation, -fov.angle / 2);
            var viewAngle02 = DirectionFromAngle(yRotation, fov.angle / 2);

            Handles.color = Color.yellow;
            Handles.DrawLine(transformPosition, transformPosition + viewAngle01 * fov.radius);
            Handles.DrawLine(transformPosition, transformPosition + viewAngle02 * fov.radius);

            if (fov.canSeePlayer)
            {
                Handles.color = Color.green;
                Handles.DrawLine(fov.transform.position, fov.playerRef.transform.position);
            }
        }

        private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
        {
            angleInDegrees += eulerY;

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}

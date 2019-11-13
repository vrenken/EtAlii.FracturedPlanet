namespace EtAlii.FracturedPlanet.World
{
    using EtAlii.FracturedPlanet.Sector;
    using UnityEngine;

    public class SectorTile : MonoBehaviour
    {
        public Sector sector;
        
        public void UpdatePositionOnWorld(
            float majorRadius, float minorRadius, 
            int majorSegmentCount, int minorSegmentCount, 
            float horizontalOffset, float verticalOffset)
        {
            var uStep = (2f * Mathf.PI) / majorSegmentCount;
            var vStep = (2f * Mathf.PI) / minorSegmentCount;

            var tileTransform = transform;
            
            var torusCenter = TorusMath.GetMajorCenter(majorRadius,sector.X * uStep);
            var tilePosition = TorusMath.GetPointOnTorus(majorRadius, minorRadius,sector.X * uStep, sector.Y * vStep);

            tileTransform.position = tilePosition;

            // Rotate around the minor axis
            var axis = Quaternion.Euler(0, 90, 0) * torusCenter;
            axis.Normalize();
            tileTransform.RotateAround(torusCenter, axis, verticalOffset);

            // Rotate around the major axis
            tilePosition = Quaternion.Euler( 0,horizontalOffset, 0) * tileTransform.position;
            torusCenter = Quaternion.Euler(0, horizontalOffset, 0) * torusCenter;

            // Apply the rotations.
            transform.position = tilePosition;

            // Apply a look at.
            var lookAtRotation = Quaternion.LookRotation(tilePosition - torusCenter);
            tileTransform.rotation = lookAtRotation;
        }
        
    }
}
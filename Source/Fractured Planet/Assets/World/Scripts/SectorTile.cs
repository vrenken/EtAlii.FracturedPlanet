namespace EtAlii.FracturedPlanet.World
{
    using EtAlii.FracturedPlanet.Sector;
    using UnityEngine;

    public class SectorTile : MonoBehaviour
    {
        public Sector sector;

        private void Start()
        {
        }

        public void UpdatePositionOnWorld(float majorRadius, float minorRadius, int majorSegmentCount, int minorSegmentCount)
        {
            var uStep = (2f * Mathf.PI) / majorSegmentCount;
            var vStep = (2f * Mathf.PI) / minorSegmentCount;

            var tileTransform = transform;
            tileTransform.position = TorusMath.GetPointOnTorus(majorRadius, minorRadius,sector.X * uStep, sector.Y * vStep);
            var torusCenter = TorusMath.GetMajorCenter(majorRadius,sector.X * uStep);
            
            tileTransform.rotation = Quaternion.LookRotation(tileTransform.position - torusCenter);
//            Gizmos.color = new Color(
//                1f,
//                (float) sector.Y / world.majorSegmentCount,
//                (float) sector.X / world.minorSegmentCount);

            //Gizmos.DrawSphere(point, 0.1f);
        }
    }
}
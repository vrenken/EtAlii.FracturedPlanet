namespace EtAlii.FracturedPlanet.World
{
    using UnityEngine;

    public static class TorusMath
    {
        public static Vector3 GetPointOnTorus(float majorRadius, float minorRadius, float u, float v)
        {
            var r = (majorRadius + minorRadius * Mathf.Cos(v));

            return new Vector3
            {
                z = r * Mathf.Sin(u),
                x = r * Mathf.Cos(u),
                y = minorRadius * Mathf.Sin(v)
            };
        }
        public static Vector3 GetMajorCenter(float majorRadius, float u)
        {
            var r = (majorRadius);

            return new Vector3
            {
                z = r * Mathf.Sin(u),
                x = r * Mathf.Cos(u),
                y = 0f
            };
        }
    }
}
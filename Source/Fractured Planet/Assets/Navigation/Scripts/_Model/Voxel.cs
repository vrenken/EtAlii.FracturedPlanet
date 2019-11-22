namespace EtAlii.FracturedPlanet.Navigation
{
    using UnityEngine;

    public struct Voxel
    {
        public Vector3Int LocalPosition;
        public float Density;

        public Voxel(Vector3Int localPosition, float density)
        {
            this.LocalPosition = localPosition;
            this.Density = density;
        }
    }
}
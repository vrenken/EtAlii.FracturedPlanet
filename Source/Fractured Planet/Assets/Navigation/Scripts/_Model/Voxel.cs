﻿namespace EtAlii.FracturedPlanet.Navigation
{
    using UnityEngine;

    public struct Voxel
    {
        public Vector3Int localPosition;
        public float density;

        public Voxel(Vector3Int localPosition, float density)
        {
            this.localPosition = localPosition;
            this.density = density;
        }
    }
}
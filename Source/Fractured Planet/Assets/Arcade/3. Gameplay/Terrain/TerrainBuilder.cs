// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

namespace EtAlii.FracturedPlanet.Terrain
{
    using UnityEngine;

    public class TerrainBuilder : MonoBehaviour
    {
        public GameObject sectorPrefab;

        public int width = 5;
        public int height = 5;
        public int sectorSize = 20;

        private void Start()
        {
            var offsetX = width * sectorSize / 2f;
            var offsetZ = height * sectorSize / 2f;

            for (var z = 0; z < height; z++)
            {
                for (var x = 0; x < width; x++)
                {
                    var sector = Instantiate(sectorPrefab, transform);

                    var position = new Vector3(x * sectorSize - offsetX, 0f, z * sectorSize - offsetZ);
                    sector.transform.position = position;
                    sector.gameObject.name = $"Sector ({x:+00;-00} x {z:+00;-00})";
                }
            }
        }
    }
}

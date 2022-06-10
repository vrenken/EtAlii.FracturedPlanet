// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

namespace EtAlii.FracturedPlanet.Terrain
{
    using System.Collections;
    using UnityEngine;

    public class TerrainBuilder : MonoBehaviour
    {
        public GameObject sectorPrefab;

        public int width = 5;
        public int height = 5;
        public int sectorSize = 20;

        public Vector3 correctiveOffset = new Vector3(-10, 0, -10);
        private void Start()
        {
            StartCoroutine(Build());
        }

        private IEnumerator Build()
        {
            var offsetX = width * sectorSize / 2f;
            var offsetZ = height * sectorSize / 2f;

            for (var z = 0; z < height; z++)
            {
                for (var x = 0; x < width; x++)
                {
                    var sector = Instantiate(sectorPrefab, transform);

                    var position = new Vector3(-offsetX - sectorSize + x * sectorSize * 2, 0f, -offsetZ - sectorSize + z * sectorSize * 2);

                    position += correctiveOffset;

                    sector.gameObject.name = $"Sector ({x:+00;-00} x {z:+00;-00})";
                    var generator = sector.GetComponent<TilesMapGenerator>();
                    generator.mapSize = sectorSize;
                    yield return generator.NewMap();
                    sector.transform.position = position;
                }
            }
        }
    }
}

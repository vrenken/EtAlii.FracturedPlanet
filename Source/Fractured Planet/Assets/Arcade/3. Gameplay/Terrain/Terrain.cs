// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

namespace EtAlii.FracturedPlanet.Terrain
{
    using System.Collections;
    using Unity.AI.Navigation;
    using UnityEngine;

    [RequireComponent(typeof(NavMeshSurface))]
    public class Terrain : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private GameObject sectorPrefab;

        [SerializeField] private Vector3 correctiveOffset = new Vector3(-10, 0, -10);

        [SerializeField] private int width = 5;
        [SerializeField] private int height = 5;
        [SerializeField] private int sectorSize = 20;

        private void Start()
        {
            navMeshSurface = GetComponent<NavMeshSurface>();
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

            navMeshSurface.BuildNavMesh();
        }
        //
        // private void AddNavMeshData()
        // {
        //     if (navMeshData != null)
        //     {
        //         if (navMeshDataInstance.valid)
        //         {
        //             NavMesh.RemoveNavMeshData(navMeshDataInstance);
        //         }
        //         navMeshDataInstance = NavMesh.AddNavMeshData(navMeshData);
        //     }
        // }
        // private List<NavMeshBuildSource> GetBuildSources()
        // {
        //     var sources = new List<NavMeshBuildSource>();
        //     NavMeshBuilder.CollectSources(GetBounds(), navMeshLayerMask, NavMeshCollectGeometry.PhysicsColliders, 0, new List<NavMeshBuildMarkup>(), sources);
        //     //Debug.LogFormat("Sources {0}", sources.Count);
        //     return sources;
        // }
        //
        // private NavMeshBuildSettings GetSettings()
        // {
        //     var settings = NavMesh.GetSettingsByID(settingsId);
        //     return settings;
        // }
        //
        // private Bounds GetBounds()
        // {
        //     return new Bounds(boundsCenter, boundsSize);
        // }
    }
}

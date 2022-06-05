// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

namespace EtAlii.FracturedPlanet.Arcade
{
    using UnityEngine;

    public class Player
    {
        public readonly PlayerType Type;

        public readonly Vector2 SpawnPoint;

        public GameObject Camera { get; private set; }
        public GameObject TrackingCamera { get; private set; }
        public GameObject Instance { get; private set; }

        public Player(Vector2 spawnPoint, PlayerType type)
        {
            SpawnPoint = spawnPoint;
            Type = type;
        }

        public static void ConfigureInstance(Player player, GameObject instance)
        {
            player.Instance = instance;
        }
        public static void ConfigureCameras(Player player, GameObject camera, GameObject trackingCamera)
        {
            player.Camera = camera;
            player.TrackingCamera = trackingCamera;
        }
    }
}
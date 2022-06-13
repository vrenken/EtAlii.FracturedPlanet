// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

// ReSharper disable once CheckNamespace
namespace EtAlii.FracturedPlanet.Arcade
{
    using UnityEngine;

    public class Player
    {
        public int PlayerNumber { get; private set; }
        public int VisiblePlayerCount { get; private set; }

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

        public static void ConfigureInstance(Player player, GameObject instance, int playerNumber, int playerCount)
        {
            player.Instance = instance;
            player.PlayerNumber = playerNumber;
            player.VisiblePlayerCount = playerCount;
        }
        public static void ConfigureCameras(Player player, GameObject camera, GameObject trackingCamera)
        {
            player.Camera = camera;
            player.TrackingCamera = trackingCamera;
        }
    }
}

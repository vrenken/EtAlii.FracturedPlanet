// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

namespace EtAlii.FracturedPlanet.Arcade
{
    using System;
    using System.Linq;
    using Cinemachine;
    using Complete;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Object = UnityEngine.Object;

    public class GameStarter
    {

        public Game StartPlayerSelection()
        {
            var players = new[]
            {
                new VisiblePlayer(WellKnownResources.Current.spawnPoints[0], PlayerType.Bot, false),
                new Player(WellKnownResources.Current.spawnPoints[1], PlayerType.Bot),
                new Player(WellKnownResources.Current.spawnPoints[2], PlayerType.Bot),
                new Player(WellKnownResources.Current.spawnPoints[3], PlayerType.Bot)
            };
            var game = Start(players);

            var overlays = GameObject.Find("Overlays");
            var playerSelectionOverlay = Object.Instantiate(WellKnownResources.Current.playerSelectionOverlayPrefab, overlays.transform);
            playerSelectionOverlay.name = "Player Selection Overlay";

            return game;
        }

        public Game Start(Player[] players)
        {
            AddPlayers(players);

            ConfigureCameras(players);

            WireCamerasToPlayers(players);

            AddOverlays(players);

            return new Game(players);
        }

        private void AddOverlays(Player[] players)
        {
            var overlays = GameObject.Find("Overlays");
            for (var i = 0; i < overlays.transform.childCount; i++)
            {
                Object.Destroy(overlays.transform.GetChild(i).gameObject);
            }

            var visiblePlayers = players
                .OfType<VisiblePlayer>()
                .ToArray();

            for (var i = 0; i < visiblePlayers.Length; i++)
            {
                if (visiblePlayers[i].ShowHud)
                {
                    var playerOverlay = Object.Instantiate(WellKnownResources.Current.gameplayOverlayPrefab, overlays.transform);
                    playerOverlay.name = $"Player {i}";

                    var panelSettings = Object.Instantiate(WellKnownResources.Current.gameplayPanelSettings);
                    panelSettings.name = $"Panel Settings (Player {i})";

                    var uiDocument = playerOverlay.gameObject.AddComponent<UIDocument>();
                    uiDocument.panelSettings = panelSettings;
                    uiDocument.visualTreeAsset = Object.Instantiate(WellKnownResources.Current.gameplayOverlayLayout);
                    uiDocument.visualTreeAsset.name = "Layout";

                    var gameplayOverlay = playerOverlay.GetComponent<GameplayOverlay>();
                    gameplayOverlay.layout = uiDocument;
                    gameplayOverlay.Initialize(visiblePlayers[i]);
                }
            }
        }

        private void WireCamerasToPlayers(Player[] players)
        {
            var visiblePlayers = players
                .OfType<VisiblePlayer>()
                .ToArray();

            for (var i = 0; i < visiblePlayers.Length; i++)
            {
                var clearShot = players[i].TrackingCamera.GetComponent<CinemachineClearShot>();
                clearShot.Follow = players[i].Instance.transform;
                clearShot.LookAt = players[i].Instance.transform;
            }
        }

        private void ConfigureCameras(Player[] players)
        {
            var cameras = GameObject.Find("Cameras");
            Object.Destroy(cameras.gameObject);

            var visiblePlayers = players
                .OfType<VisiblePlayer>()
                .ToArray();

            var cameraSetup = visiblePlayers.Length switch
            {
                1 => WellKnownResources.Current.onePlayerCameraSetup,
                2 => WellKnownResources.Current.twoPlayerCameraSetup,
                3 => WellKnownResources.Current.threePlayerCameraSetup,
                4 => WellKnownResources.Current.fourPlayerCameraSetup,
                _ => throw new ArgumentOutOfRangeException()
            };
            var cameraSetupInstance = Object.Instantiate(cameraSetup.prefab);
            cameraSetupInstance.name = "Cameras";
            cameraSetupInstance.SetActive(true);

            for (var i = 1; i <= visiblePlayers.Length; i++)
            {
                var camera = cameraSetupInstance.transform.Find($"Player {i} Camera");
                var trackingCamera = cameraSetupInstance.transform.Find($"Player {i} Tracking Camera");
                Player.ConfigureCameras(visiblePlayers[i - 1], camera.gameObject, trackingCamera.gameObject);
            }
        }


        private void AddPlayers(Player[] players)
        {
            var playersRoot = GameObject.Find("Players");
            for (var i = 0; i < playersRoot.transform.childCount; i++)
            {
                Object.Destroy(playersRoot.transform.GetChild(i).gameObject);
            }

            var human = 1;
            var bot = 1;

            for(var i = 0; i < players.Length; i++)
            {
                var player = players[i];
                var startPosition = new Vector3(player.SpawnPoint.x, 1, player.SpawnPoint.y);

                GameObject playerInstance;
                switch (player.Type)
                {
                    case PlayerType.Bot:
                        playerInstance = Object.Instantiate(WellKnownResources.Current.botPlayerPrefab, startPosition, Quaternion.identity, playersRoot.transform);
                        playerInstance.name = $"Bot {bot++}";
                        break;
                    case PlayerType.Human:
                        playerInstance = Object.Instantiate(WellKnownResources.Current.humanPlayerPrefab, startPosition, Quaternion.identity, playersRoot.transform);
                        playerInstance.name = $"Human {human++}";
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                var playerNumber = i + 1;

                Player.ConfigureInstance(player, playerInstance, playerNumber, players.OfType<VisiblePlayer>().Count());

                playerInstance.transform.Translate(0f, 15f, 0f); // Move it up a bit.
                playerInstance.GetComponent<PlayerController>().player = player;
            }
        }
    }
}

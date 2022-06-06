// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

using System;
using EtAlii.FracturedPlanet.Arcade;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Fracture/WellKnownResources")]
public class WellKnownResources : ScriptableObject
{
    public CameraSetup onePlayerCameraSetup;
    public CameraSetup twoPlayerCameraSetup;
    public CameraSetup threePlayerCameraSetup;
    public CameraSetup fourPlayerCameraSetup;

    public GameObject humanPlayerPrefab;
    public GameObject botPlayerPrefab;

    public GameObject settingsOverlayPrefab;
    public GameObject highScoresOverlayPrefab;

    public GameObject playerSelectionOverlayPrefab;

    public GameObject mapSelectionOverlayPrefab;

    public GameObject gameplayOverlayPrefab;
    public VisualTreeAsset gameplayOverlayLayout;
    public PanelSettings gameplayPanelSettings;

    public GameObject gameOverOverlayPrefab;
    public GameObject scoresOverlayPrefab;

    public Vector2[] spawnPoints = new Vector2[4];
    public static WellKnownResources Current => _current.Value;
    private static readonly Lazy<WellKnownResources> _current = new Lazy<WellKnownResources>(() => Resources.Load<WellKnownResources>("WellKnownResources")) ;
}

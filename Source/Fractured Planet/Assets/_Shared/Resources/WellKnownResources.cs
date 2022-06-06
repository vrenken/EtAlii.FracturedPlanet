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

    public GameObject playerScreenPrefab;
    public GameObject settingsScreenPrefab;
    public GameObject gameplayScreenPrefab;
    public VisualTreeAsset gameplayScreenLayout;
    public GameObject gameOverScreenPrefab;
    public GameObject highScoresScreenPrefab;
    public GameObject scoresScreenPrefab;
    public GameObject mapScreenPrefab;

    public Vector2[] spawnPoints = new Vector2[4];
    public static WellKnownResources Current => _current.Value;
    private static readonly Lazy<WellKnownResources> _current = new Lazy<WellKnownResources>(() => Resources.Load<WellKnownResources>("WellKnownResources")) ;
}

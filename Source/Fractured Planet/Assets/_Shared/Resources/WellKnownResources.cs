// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

namespace EtAlii.FracturedPlanet._Shared.Resources
{
    using System;
    using EtAlii.FracturedPlanet.Arcade;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Assets/WellKnownResources")]
    public class WellKnownResources : ScriptableObject
    {
        public CameraSetup onePlayerCameraSetup;
        public CameraSetup twoPlayerCameraSetup;
        public CameraSetup threePlayerCameraSetup;
        public CameraSetup fourPlayerCameraSetup;

        public static WellKnownResources Current => _current.Value;
        private static readonly Lazy<WellKnownResources> _current = new Lazy<WellKnownResources>(() => Resources.Load<WellKnownResources>("WellKnownResources")) ;
    }
}

// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

namespace EtAlii.FracturedPlanet.Arcade
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Game/Assets/CameraSetup")]
    public class CameraSetup : ScriptableObject
    {
        public GameObject prefab;
        public Camera[] cameras;
        public GameObject[] trackingCameras;
    }
}

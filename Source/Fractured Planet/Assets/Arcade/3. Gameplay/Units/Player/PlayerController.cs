// ReSharper disable All

namespace Complete
{
    using System;
    using EtAlii.FracturedPlanet.Arcade;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(CharacterSkinController))]
    public class PlayerController : MonoBehaviour
    {
        public Player player;

        public GameObject CameraRoot;
        public CharacterSkinController characterSkinController;
    }
}

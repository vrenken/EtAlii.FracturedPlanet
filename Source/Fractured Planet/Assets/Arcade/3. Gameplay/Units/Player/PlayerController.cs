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

        public CharacterSkinController characterSkinController;

        private void Start()
        {
            characterSkinController = GetComponent<CharacterSkinController>();
            characterSkinController.ChangeMaterialSettings(player.PlayerNumber);
        }
    }
}

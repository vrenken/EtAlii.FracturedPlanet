// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

namespace EtAlii.FracturedPlanet.Arcade
{
    using UnityEngine;

    public class VisiblePlayer : Player
    {
        public readonly bool ShowHud;

        public VisiblePlayer(Vector2 spawnPoint, PlayerType type, bool showHud)
        : base(spawnPoint, type)
        {
            ShowHud = showHud;

        }
    }
}

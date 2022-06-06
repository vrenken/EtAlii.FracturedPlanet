// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

using System;
using EtAlii.FracturedPlanet.Arcade;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayOverlay : MonoBehaviour
{
    public UIDocument layout;

    private VisualElement _root;

    private Player _player;

    protected void OnEnable()
    {
        if (_player != null)
        {
            ApplyLayout();
        }
    }

    public void Initialize(Player player)
    {
        _player = player;
        ApplyLayout();
    }

    private void ApplyLayout()
    {
        var root = layout.rootVisualElement.parent.Q<VisualElement>("Root");

        var quitButton = root.Q<Button>("QuitButton");
        quitButton.clickable.clicked += OnQuit;

        root.style.left = (PlayerCount: _player.VisiblePlayerCount, _player.PlayerNumber) switch
        {
            (1, 1) => new StyleLength(new Length(00, LengthUnit.Percent)),
            (2, 1) => new StyleLength(new Length(00, LengthUnit.Percent)),
            (2, 2) => new StyleLength(new Length(50, LengthUnit.Percent)),
            (3, 1) => new StyleLength(new Length(00, LengthUnit.Percent)),
            (3, 2) => new StyleLength(new Length(33, LengthUnit.Percent)),
            (3, 3) => new StyleLength(new Length(67, LengthUnit.Percent)),
            (4, 1) => new StyleLength(new Length(00, LengthUnit.Percent)),
            (4, 2) => new StyleLength(new Length(50, LengthUnit.Percent)),
            (4, 3) => new StyleLength(new Length(00, LengthUnit.Percent)),
            (4, 4) => new StyleLength(new Length(50, LengthUnit.Percent)),
            _ => throw new InvalidOperationException()
        };
        root.style.top = (PlayerCount: _player.VisiblePlayerCount, _player.PlayerNumber) switch
        {
            (1, 1) => new StyleLength(new Length(00, LengthUnit.Percent)),
            (2, 1) => new StyleLength(new Length(00, LengthUnit.Percent)),
            (2, 2) => new StyleLength(new Length(00, LengthUnit.Percent)),
            (3, 1) => new StyleLength(new Length(00, LengthUnit.Percent)),
            (3, 2) => new StyleLength(new Length(00, LengthUnit.Percent)),
            (3, 3) => new StyleLength(new Length(00, LengthUnit.Percent)),
            (4, 1) => new StyleLength(new Length(00, LengthUnit.Percent)),
            (4, 2) => new StyleLength(new Length(00, LengthUnit.Percent)),
            (4, 3) => new StyleLength(new Length(50, LengthUnit.Percent)),
            (4, 4) => new StyleLength(new Length(50, LengthUnit.Percent)),
            _ => throw new InvalidOperationException()
        };
        root.style.width = (PlayerCount: _player.VisiblePlayerCount, _player.PlayerNumber) switch
        {
            (1, 1) => new StyleLength(new Length(100, LengthUnit.Percent)),
            (2, 1) => new StyleLength(new Length(50, LengthUnit.Percent)),
            (2, 2) => new StyleLength(new Length(50, LengthUnit.Percent)),
            (3, 1) => new StyleLength(new Length(33, LengthUnit.Percent)),
            (3, 2) => new StyleLength(new Length(34, LengthUnit.Percent)),
            (3, 3) => new StyleLength(new Length(33, LengthUnit.Percent)),
            (4, 1) => new StyleLength(new Length(50, LengthUnit.Percent)),
            (4, 2) => new StyleLength(new Length(50, LengthUnit.Percent)),
            (4, 3) => new StyleLength(new Length(50, LengthUnit.Percent)),
            (4, 4) => new StyleLength(new Length(50, LengthUnit.Percent)),
            _ => throw new InvalidOperationException()
        };
        root.style.height = (PlayerCount: _player.VisiblePlayerCount, _player.PlayerNumber) switch
        {
            (1, 1) => new StyleLength(new Length(100, LengthUnit.Percent)),
            (2, 1) => new StyleLength(new Length(100, LengthUnit.Percent)),
            (2, 2) => new StyleLength(new Length(100, LengthUnit.Percent)),
            (3, 1) => new StyleLength(new Length(100, LengthUnit.Percent)),
            (3, 2) => new StyleLength(new Length(100, LengthUnit.Percent)),
            (3, 3) => new StyleLength(new Length(100, LengthUnit.Percent)),
            (4, 1) => new StyleLength(new Length(50, LengthUnit.Percent)),
            (4, 2) => new StyleLength(new Length(50, LengthUnit.Percent)),
            (4, 3) => new StyleLength(new Length(50, LengthUnit.Percent)),
            (4, 4) => new StyleLength(new Length(50, LengthUnit.Percent)),
            _ => throw new InvalidOperationException()
        };
    }

    private void OnQuit()
    {
        new GameStarter().StartPlayerSelection();
    }
}

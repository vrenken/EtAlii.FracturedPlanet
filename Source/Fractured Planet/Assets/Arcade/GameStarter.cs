// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

namespace EtAlii.FracturedPlanet.Arcade
{
    public class GameStarter
    {
        public Game Start(int playerCount)
        {
            var players = new Player[playerCount];

            for (var i = 0; i < playerCount; i++)
            {
                players[i] = new Player();
            }

            return new Game(players);
        }
    }
}

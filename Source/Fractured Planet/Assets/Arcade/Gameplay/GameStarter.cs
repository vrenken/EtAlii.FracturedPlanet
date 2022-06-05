// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

namespace EtAlii.FracturedPlanet.Arcade
{
    using System.Linq;

    public class GameStarter
    {

        public Game Start(Player[] visiblePlayers, Player[] invisiblePlayers)
        {

            switch (visiblePlayers.Length)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }

            // var players = new Player[playerCount];
            //
            // for (var i = 0; i < playerCount; i++)
            // {
            //     players[i] = new Player();
            // }

            var players = visiblePlayers
                .Concat(invisiblePlayers)
                .ToArray();

            return new Game(players);
        }
    }
}

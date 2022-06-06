using UnityEngine;

namespace EtAlii.FracturedPlanet
{
    using EtAlii.FracturedPlanet.Arcade;

    public class PlayerScreen : MonoBehaviour
    {
        public void StartOnePlayerGame()
        {
            var starter = new GameStarter();

            var players = new[]
            {
                new VisiblePlayer(WellKnownResources.Current.spawnPoints[0], PlayerType.Human, true),
                new Player(WellKnownResources.Current.spawnPoints[1], PlayerType.Bot),
                new Player(WellKnownResources.Current.spawnPoints[2], PlayerType.Bot),
                new Player(WellKnownResources.Current.spawnPoints[3], PlayerType.Bot)
            };
            starter.Start(players);

            Destroy(gameObject);
        }
        public void StartTwoPlayerGame()
        {
            var starter = new GameStarter();

            var players = new[]
            {
                new VisiblePlayer(WellKnownResources.Current.spawnPoints[0], PlayerType.Human, true),
                new VisiblePlayer(WellKnownResources.Current.spawnPoints[1], PlayerType.Human, true),
                new Player(WellKnownResources.Current.spawnPoints[2], PlayerType.Bot),
                new Player(WellKnownResources.Current.spawnPoints[3], PlayerType.Bot)
            };
            starter.Start(players);

            Destroy(gameObject);
        }
        public void StartThreePlayerGame()
        {
            var starter = new GameStarter();

            var players = new[]
            {
                new VisiblePlayer(WellKnownResources.Current.spawnPoints[0], PlayerType.Human, true),
                new VisiblePlayer(WellKnownResources.Current.spawnPoints[1], PlayerType.Human, true),
                new VisiblePlayer(WellKnownResources.Current.spawnPoints[2], PlayerType.Human, true),
                new Player(WellKnownResources.Current.spawnPoints[3], PlayerType.Bot)
            };
            starter.Start(players);

            Destroy(gameObject);
        }
        public void StartFourPlayerGame()
        {
            var starter = new GameStarter();

            var players = new[]
            {
                new VisiblePlayer(WellKnownResources.Current.spawnPoints[0], PlayerType.Human, true),
                new VisiblePlayer(WellKnownResources.Current.spawnPoints[1], PlayerType.Human, true),
                new VisiblePlayer(WellKnownResources.Current.spawnPoints[2], PlayerType.Human, true),
                new VisiblePlayer(WellKnownResources.Current.spawnPoints[3], PlayerType.Human, true)
            };
            starter.Start(players);

            Destroy(gameObject);
        }
    }
}

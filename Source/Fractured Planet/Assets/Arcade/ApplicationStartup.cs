using UnityEngine;
using EtAlii.FracturedPlanet.Arcade;

public class ApplicationStartup : MonoBehaviour
{
    private void Start()
    {
        var starter = new GameStarter();

        var players = new[]
        {
            new VisiblePlayer(WellKnownResources.Current.spawnPoints[0], PlayerType.Bot, false),
            new Player(WellKnownResources.Current.spawnPoints[1], PlayerType.Bot),
            new Player(WellKnownResources.Current.spawnPoints[2], PlayerType.Bot),
            new Player(WellKnownResources.Current.spawnPoints[3], PlayerType.Bot)
        };
        starter.Start(players);

        var playerScreen = Instantiate(WellKnownResources.Current.playerScreenPrefab);
        playerScreen.name = "Player Selection Screen";

        Destroy(this);
    }

}

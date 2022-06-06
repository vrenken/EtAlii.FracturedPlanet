using UnityEngine;

using EtAlii.FracturedPlanet.Arcade;
using UnityEngine.UIElements;

public class PlayerSelectionScreen : MonoBehaviour
{
    public UIDocument layout;

    private void OnEnable()
    {
        var onePlayerButton = layout.rootVisualElement.parent.Q<Button>("OnePlayerButton");
        onePlayerButton.clickable.clicked += StartOnePlayerGame;

        var twoPlayerButton = layout.rootVisualElement.parent.Q<Button>("TwoPlayerButton");
        twoPlayerButton.clickable.clicked += StartTwoPlayerGame;

        var threePlayerButton = layout.rootVisualElement.parent.Q<Button>("ThreePlayerButton");
        threePlayerButton.clickable.clicked += StartThreePlayerGame;

        var fourPlayerButton = layout.rootVisualElement.parent.Q<Button>("FourPlayerButton");
        fourPlayerButton.clickable.clicked += StartFourPlayerGame;
    }

    private void StartOnePlayerGame()
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

    private void StartTwoPlayerGame()
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

    private void StartThreePlayerGame()
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

    private void StartFourPlayerGame()
    {
        var starter = new GameStarter();

        var players = new Player[]
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

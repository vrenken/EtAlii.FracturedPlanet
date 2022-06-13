using UnityEngine;
using EtAlii.FracturedPlanet.Arcade;

// ReSharper disable once CheckNamespace
public class ApplicationStartup : MonoBehaviour
{
    private void Start()
    {
        var starter = new GameStarter();

        starter.StartPlayerSelection();

        DestroyImmediate(gameObject);
    }

}

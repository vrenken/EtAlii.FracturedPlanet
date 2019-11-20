namespace EtAlii.FracturedPlanet.Navigation
{
    using UnityEngine;

    public static partial class LookupTables
    {

        public static readonly Vector3Int[] CubePoints =
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, 0, 1),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, 1, 1),
            new Vector3Int(0, 1, 1)
        };

        public static readonly int[] CubePointsX =
        {
            0,
            1,
            1,
            0,
            0,
            1,
            1,
            0
        };

        public static readonly int[] CubePointsY =
        {
            0,
            0,
            0,
            0,
            1,
            1,
            1,
            1
        };

        public static readonly int[] CubePointsZ =
        {
            0,
            0,
            1,
            1,
            0,
            0,
            1,
            1
        };
    }
}
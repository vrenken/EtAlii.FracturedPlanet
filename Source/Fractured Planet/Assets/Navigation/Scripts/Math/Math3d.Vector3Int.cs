using UnityEngine;

public static partial class Math3d
{
    
    public static Vector3Int Abs(Vector3Int n)
    {
        var x = Abs(n.x);
        var y = Abs(n.y);
        var z = Abs(n.z);

        return new Vector3Int(x, y, z);
    }

    public static Vector3Int FloorToNearestX(Vector3Int n, int x)
    {
        var x1 = FloorToNearestX(n.x, x);
        var y1 = FloorToNearestX(n.y, x);
        var z1 = FloorToNearestX(n.z, x);

        return new Vector3Int(x1, y1, z1);
    }

    public static Vector3Int Mod(Vector3Int n, int x)
    {
        var x1 = Mod(n.x, x);
        var y1 = Mod(n.y, x);
        var z1 = Mod(n.z, x);

        return new Vector3Int(x1, y1, z1);
    }

}

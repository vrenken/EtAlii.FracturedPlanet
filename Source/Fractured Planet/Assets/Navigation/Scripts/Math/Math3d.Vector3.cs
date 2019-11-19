using UnityEngine;

public static partial class Math3d
{
    
    public static Vector3Int CeilToNearestX(Vector3 n, int x)
    {
        var x1 = CeilToNearestX(n.x, x);
        var y1 = CeilToNearestX(n.y, x);
        var z1 = CeilToNearestX(n.z, x);

        return new Vector3Int(x1, y1, z1);
    }

    public static Vector3Int Floor(Vector3 n)
    {
        var x = Floor(n.x);
        var y = Floor(n.y);
        var z = Floor(n.z);

        return new Vector3Int(x, y, z);
    }

    public static Vector3Int Ceil(Vector3 n)
    {
        var x = Ceil(n.x);
        var y = Ceil(n.y);
        var z = Ceil(n.z);

        return new Vector3Int(x, y, z);
    }

    public static Vector3Int Round(Vector3 n)
    {
        var x = Round(n.x);
        var y = Round(n.y);
        var z = Round(n.z);

        return new Vector3Int(x, y, z);
    }
    public static Vector3Int RoundToNearestX(Vector3 n, int x)
    {
        var x1 = RoundToNearestX(n.x, x);
        var y1 = RoundToNearestX(n.y, x);
        var z1 = RoundToNearestX(n.z, x);

        return new Vector3Int(x1, y1, z1);
    }

    public static Vector3Int FloorToNearestX(Vector3 n, int x)
    {
        var x1 = FloorToNearestX(n.x, x);
        var y1 = FloorToNearestX(n.y, x);
        var z1 = FloorToNearestX(n.z, x);

        return new Vector3Int(x1, y1, z1);
    }

    public static float Distance(Vector3 p1, Vector3 p2)
    {
        var sqrD = SqrDistance(p1, p2);
        var result = Mathf.Sqrt(sqrD);

        return result;
    }

    public static Vector3 Abs(Vector3 n)
    {
        var x = Abs(n.x);
        var y = Abs(n.y);
        var z = Abs(n.z);

        return new Vector3(x, y, z);
    }
    
    public static float SqrDistance(Vector3 p1, Vector3 p2)
    {
        var p3 = p1 - p2;

        var x = p3.x;
        var y = p3.y;
        var z = p3.z;

        var result = x * x + y * y + z * z;

        return result;
    }
}

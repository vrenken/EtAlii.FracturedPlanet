using UnityEngine;

public static partial class Math3d
{
    public static float Abs(float n)
    {
        return n < 0 ? -n : n;
    }

    public static int Floor(float n)
    {
        return (int)Mathf.Floor(n);
    }

    public static int Ceil(float n)
    {
        return Floor(n) + 1;
    }

    public static int Round(float n)
    {
        var floored = Floor(n);

        var diff = n - floored;
        if (diff < 0.5f)
            return floored;
        else
            return floored + 1;
    }

    public static int RoundToNearestX(float n, int x)
    {
        return Round(n / x) * x;
    }

    public static int FloorToNearestX(float n, int x)
    {
        return Floor(n / x) * x;
    }

    public static int CeilToNearestX(float n, int x)
    {
        return Ceil(n / x) * x;
    }
    
    
    public static float Clamp(float number, float min, float max)
    {
        if (number < min)
            return min;
        else if (number > max)
            return max;
        else
            return number;
    }

    public static float Clamp01(float number)
    {
        return Clamp(number, 0, 1);
    }

    public static float Max(float a, float b)
    {
        return a > b ? a : b;
    }

    public static float Min(float a, float b)
    {
        return a < b ? a : b;
    }

    
    public static float SqrDistance(float x, float y, float z, Vector3 p1)
    {
        var x1 = x - p1.x;
        var y1 = y - p1.y;
        var z1 = z - p1.z;

        var result = x1 * x1 + y1 * y1 + z1 * z1;

        return result;
    }

    public static float Distance(float x, float y, float z, Vector3 p2)
    {
        var sqrD = SqrDistance(x, y, z, p2);
        var result = Mathf.Sqrt(sqrD);

        return result;
    }

    public static float Map(float value, float x1, float y1, float x2, float y2)
    {
        return (value - x1) / (y1 - x1) * (y2 - x2) + x2;
    }
}

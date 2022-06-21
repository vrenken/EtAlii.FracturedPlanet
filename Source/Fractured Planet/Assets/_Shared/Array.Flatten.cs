// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

namespace EtAlii.FracturedPlanet
{
    public static class ArrayFlattenExtension
    {
        public static T[] Flatten<T>(this T[,] items)
        {
            var size = items.Length;
            var result = new T[size];

            // Step 2: copy 2D array elements into a 1D array.
            var write = 0;
            for (var y = 0; y <= items.GetUpperBound(0); y++)
            {
                for (var x = 0; x <= items.GetUpperBound(1); x++)
                {
                    result[write++] = items[y, x];
                }
            }

            // Step 3: return the new array.
            return result;
        }

        public static (int X, int Y, T Item)[] FlattenWithCoordinates<T>(this T[,] items)
        {
            var size = items.Length;
            var result = new (int X, int Y, T Item)[size];

            // Step 2: copy 2D array elements into a 1D array.
            var write = 0;
            for (var y = 0; y <= items.GetUpperBound(0); y++)
            {
                for (var x = 0; x <= items.GetUpperBound(1); x++)
                {
                    result[write++] = new(x, y, items[y, x]);
                }
            }

            // Step 3: return the new array.
            return result;
        }
    }
}

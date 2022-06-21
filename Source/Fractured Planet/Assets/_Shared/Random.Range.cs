// Copyright (c) Peter Vrenken. All rights reserved. See the license on https://github.com/vrenken/EtAlii.RemoteMesh

namespace EtAlii.FracturedPlanet
{
    using System;

    public static class RandomRangeExtensions
    {
        public static float Range(this Random random, float min, float max)
        {
            var value = (max - min) / 1f * (float)random.NextDouble();
            return min + value;
        }
        public static int Range(this Random random, int min, int max)
        {
            return random.Next(min, max);
        }
    }
}

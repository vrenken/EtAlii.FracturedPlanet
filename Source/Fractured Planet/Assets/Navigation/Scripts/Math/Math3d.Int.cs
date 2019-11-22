namespace EtAlii.FracturedPlanet.Navigation
{
    public static partial class Math3d
    {
        public static readonly int[] PowerOfTwos =
        {
            1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192,
        };

        public static int Abs(int n)
        {
            return (float) n < 0f ? -n : n;
        }

        public static int Mod(int n, int x)
        {
            return (n % x + x) % x;
        }



        public static bool IsBetween(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        public static float Pow(int @base, int exponent)
        {
            var exponentNegative = exponent < 0;

            if (exponentNegative)
                exponent = -exponent;

            var result = 1;

            for (var i = 0; i < exponent; i++)
            {
                result *= @base;
            }

            if (exponentNegative)
                return 1f / result;
            else
                return result;
        }

        public static int Clamp(int number, int min, int max)
        {
            if (number < min)
                return min;
            else if (number > max)
                return max;
            else
                return number;
        }
    }
}
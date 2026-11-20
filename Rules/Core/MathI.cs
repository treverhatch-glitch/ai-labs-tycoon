namespace AiLabs.Rules.Core
{
    /// <summary>
    /// Whole-number maths. Every quantity in the rules is an integer: money in whole dollars,
    /// rates in permille (1000 = 100%), market share in parts per million. Division rounds toward zero.
    /// </summary>
    public static class MathI
    {
        public const int Permille = 1000;
        public const int Ppm = 1000000;

        public static long MulPermille(long value, long permille) { return value * permille / Permille; }

        public static long Clamp(long value, long min, long max) { return value < min ? min : value > max ? max : value; }

        public static int Clamp(int value, int min, int max) { return value < min ? min : value > max ? max : value; }

        public static long Min(long a, long b) { return a < b ? a : b; }

        public static long Max(long a, long b) { return a > b ? a : b; }

        /// <summary>Division that rounds down, also for negative numbers (plain division rounds toward zero).</summary>
        public static long FloorDiv(long a, long b)
        {
            long q = a / b;
            return (a % b != 0 && (a < 0) != (b < 0)) ? q - 1 : q;
        }

        /// <summary>a * b / c without overflow for the sizes the rules use, and 0 when c is 0.</summary>
        public static long MulDiv(long a, long b, long c) { return c == 0 ? 0 : a * b / c; }
    }
}

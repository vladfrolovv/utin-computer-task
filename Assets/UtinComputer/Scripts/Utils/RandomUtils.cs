using Random = System.Random;
namespace UtinComputer.Utils
{
    public static class RandomUtils
    {
        public static float NextRange(this Random random, float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        public static float NextNormalized(this Random random)
        {
            return (float)random.NextDouble();
        }
    }
}

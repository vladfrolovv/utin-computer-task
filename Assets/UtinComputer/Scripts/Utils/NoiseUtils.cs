using UnityEngine;
using Random = System.Random;
namespace UtinComputer.Utils
{
    public static class NoiseUtils
    {
        private const float OriginRange = 10000f;

        public static Vector2 NextOrigin(this Random random)
        {
            return new Vector2(random.NextRange(0f, OriginRange), random.NextRange(0f, OriginRange));
        }

        public static float Sample(this Vector2 origin, float x, float y, float scale)
        {
            return Mathf.PerlinNoise(origin.x + x * scale, origin.y + y * scale);
        }
    }
}

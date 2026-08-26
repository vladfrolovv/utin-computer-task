using UnityEngine;
namespace UtinComputer.Utils
{
    public static class MathUtils
    {
        private const float SphereVolumeFactor = 4f / 3f * Mathf.PI;

        public static float ToSphereVolume(this float radius)
        {
            return SphereVolumeFactor * radius * radius * radius;
        }

        public static float ToSphereRadius(this float volume)
        {
            return Mathf.Pow(Mathf.Max(volume, 0f) / SphereVolumeFactor, 1f / 3f);
        }
    }
}

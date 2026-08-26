using UnityEngine;
using UtinComputer.Utils;
namespace UtinComputer.Core
{
    public readonly struct BlastInfo
    {
        public BlastInfo(Vector3 origin, float radius, float duration)
        {
            Origin = origin;
            Radius = radius;
            Duration = duration;
        }

        public Vector3 Origin { get; }
        public float Radius { get; }
        public float Duration { get; }

        public float DelayAt(Vector3 position)
        {
            if (Radius <= 0f)
                return 0f;

            float distance = (position.Flat() - Origin.Flat()).magnitude;

            return Mathf.Clamp01(distance / Radius) * Duration;
        }
    }
}

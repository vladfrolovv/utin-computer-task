using UnityEngine;
namespace UtinComputer.Map
{
    public readonly struct TreeInfo
    {
        public TreeInfo(Vector3 position, float bodyScale, float rotation, float variant)
        {
            Position = position;
            BodyScale = bodyScale;
            Rotation = rotation;
            Variant = variant;
        }

        public Vector3 Position { get; }
        public float BodyScale { get; }
        public float Rotation { get; }
        public float Variant { get; }
    }
}

using UnityEngine;
namespace UtinComputer.Cameras
{
    [CreateAssetMenu(menuName = "UtinComputer/Camera Config", fileName = "CameraConfig")]
    public class CameraConfig : ScriptableObject
    {
        [field: SerializeField, Min(0f)] public float FollowSmoothing { get; private set; } = 6f;
        [field: SerializeField, Min(0f)] public float ShotFollowSmoothing { get; private set; } = 12f;
    }
}

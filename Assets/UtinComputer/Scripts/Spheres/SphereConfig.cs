using UnityEngine;
namespace UtinComputer.Spheres
{
    [CreateAssetMenu(menuName = "UtinComputer/Sphere Config", fileName = "SphereConfig")]
    public class SphereConfig : ScriptableObject
    {
        [field: Header("Sphere")]
        [field: SerializeField, Min(0f)] public float StartRadius { get; private set; } = 5f;
        [field: SerializeField, Min(0f)] public float MinRadius { get; private set; } = .35f;
        [field: SerializeField] public Vector3 ShootDirection { get; private set; } = Vector3.forward;

        [field: Header("Charge")]
        [field: SerializeField, Min(0f)] public float ChargeRatePerSecond { get; private set; } = .275f;
        [field: SerializeField, Min(0f)] public float ChargeAccelerationTime { get; private set; } = .35f;

        [field: Header("Shot Growth")]
        [field: SerializeField, Min(0f)] public float EmergeTime { get; private set; } = .2f;
        [field: SerializeField, Min(0f)] public float StretchPerGrowthSpeed { get; private set; } = .24f;
        [field: SerializeField, Range(0f, 1f)] public float MaxStretch { get; private set; } = .45f;
        [field: SerializeField, Min(0f)] public float StretchSmoothing { get; private set; } = 18f;

        [field: Header("Shot Release")]
        [field: SerializeField, Min(0f)] public float ReleaseKickRatio { get; private set; } = 1.6f;
        [field: SerializeField, Min(0f)] public float ReleaseRoundTime { get; private set; } = .16f;
        [field: SerializeField, Min(0f)] public float ReleaseVanishTime { get; private set; } = .18f;

        [field: Header("Sphere Feedback")]
        [field: SerializeField, Min(0f)] public float ShakeAmplitudeRatio { get; private set; } = .035f;
        [field: SerializeField, Min(0f)] public float ShakeFrequency { get; private set; } = 28f;
        [field: SerializeField, Range(.05f, 1f)] public float ShakeRampPower { get; private set; } = .3f;
        [field: SerializeField, Min(0f)] public float RecoilRatio { get; private set; } = .14f;
        [field: SerializeField, Min(0f)] public float RecoilReturnTime { get; private set; } = .3f;
        [field: SerializeField, Min(0f)] public float ReleasePunchRatio { get; private set; } = .22f;
        [field: SerializeField, Min(0f)] public float ReleasePunchTime { get; private set; } = .5f;
        [field: SerializeField, Min(0f)] public float LoseCollapseTime { get; private set; } = .45f;
    }
}

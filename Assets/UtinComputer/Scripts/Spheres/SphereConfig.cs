using UnityEngine;
namespace UtinComputer.Spheres
{
    [CreateAssetMenu(menuName = "UtinComputer/Sphere Config", fileName = "SphereConfig")]
    public class SphereConfig : ScriptableObject
    {
        [field: SerializeField] public float StartRadius { get; private set; } = 1f;
        [field: SerializeField] public float MinRadius { get; private set; } = .35f;
        [field: SerializeField] public float ChargeVolumePerSecond { get; private set; } = 2.5f;
        [field: SerializeField] public float ShakeAmplitude { get; private set; } = .12f;
        [field: SerializeField] public float ShakeFrequency { get; private set; } = 24f;
        [field: SerializeField] public float LoseCollapseTime { get; private set; } = .3f;
    }
}

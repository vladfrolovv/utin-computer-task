using UnityEngine;
namespace UtinComputer.Map
{
    [CreateAssetMenu(menuName = "UtinComputer/Map Config", fileName = "MapConfig")]
    public class MapConfig : ScriptableObject
    {
        [Tooltip("The same seed always builds the same forest.")]
        [field: SerializeField] public int Seed { get; private set; } = 1;

        [Header("Shape")]
        [Tooltip("Distance in units from the center to the edge of the forest.")]
        [field: SerializeField] public float MapRadius { get; private set; } = 250f;
        [Tooltip("Distance in units between two neighbour trees.")]
        [field: SerializeField] public float TreeSpacing { get; private set; } = 5f;
        [Tooltip("How far in units a tree can wander off its slot, so the forest does not read as a grid.")]
        [field: SerializeField] public float SpacingRandomness { get; private set; } = 1f;
        [Tooltip("Radius in units of the empty area kept around the center.")]
        [field: SerializeField] public float ClearingRadius { get; private set; } = 20f;

        [Header("Density")]
        [Tooltip("Share of slots that get a tree. 0 is empty, 1 is solid forest.")]
        [Range(0f, 1f)]
        [field: SerializeField] public float Density { get; private set; } = .8f;
        [Tooltip("Size in units of one grove or clearing. Around TreeSpacing every tree decides on its own, higher values grow wide groves.")]
        [field: SerializeField] public float ForestPatchSize { get; private set; } = 5f;

        [Header("Trees")]
        [Tooltip("Smallest tree scale.")]
        [field: SerializeField] public float MinTreeScale { get; private set; } = 2f;
        [Tooltip("Biggest tree scale.")]
        [field: SerializeField] public float MaxTreeScale { get; private set; } = 4f;
        [Tooltip("Trees preallocated per tree prefab. Below what the forest needs the rest is spawned during play.")]
        [field: SerializeField] public int PooledTreesPerPrefab { get; private set; } = 256;
    }
}

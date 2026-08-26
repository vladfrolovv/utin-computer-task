using UnityEngine;
namespace UtinComputer.Finish
{
    [CreateAssetMenu(menuName = "UtinComputer/Finish Config", fileName = "FinishConfig")]
    public class FinishConfig : ScriptableObject
    {
        [field: Header("Door")]
        [field: Tooltip("Distance in units from the start to the door, measured along the shoot direction.")]
        [field: SerializeField, Min(0f)] public float DoorDistance { get; private set; } = 140f;
        [field: Tooltip("Radius in units of the empty area kept around the door.")]
        [field: SerializeField, Min(0f)] public float DoorClearingRadius { get; private set; } = 18f;
        [field: Tooltip("Distance in units at which the door opens in front of the approaching sphere.")]
        [field: SerializeField, Min(0f)] public float DoorOpenDistance { get; private set; } = 5f;
        [field: Tooltip("Angle in degrees each leaf swings out to.")]
        [field: SerializeField, Min(0f)] public float DoorOpenAngle { get; private set; } = 100f;
        [field: Tooltip("Seconds the leaves take to swing open.")]
        [field: SerializeField, Min(0f)] public float DoorOpenTime { get; private set; } = .6f;
        [field: Tooltip("Seconds the leaves take to swing shut behind the sphere.")]
        [field: SerializeField, Min(0f)] public float DoorCloseTime { get; private set; } = .5f;

        [field: Header("Advance")]
        [field: Tooltip("Extra half width in units the corridor needs on top of the sphere radius.")]
        [field: SerializeField, Min(0f)] public float CorridorClearance { get; private set; } = 1.5f;
        [field: Tooltip("Gap in units the sphere keeps to the first tree it cannot pass.")]
        [field: SerializeField, Min(0f)] public float BlockerGap { get; private set; } = 1.5f;
        [field: Tooltip("Shortest advance in units worth animating. Below this the sphere stays put and the player shoots again.")]
        [field: SerializeField, Min(0f)] public float MinAdvanceDistance { get; private set; } = 2f;

        [field: Header("Jump")]
        [field: Tooltip("Length in units of a single hop.")]
        [field: SerializeField, Min(.1f)] public float HopDistance { get; private set; } = 8f;
        [field: Tooltip("Seconds one hop takes.")]
        [field: SerializeField, Min(.01f)] public float HopTime { get; private set; } = .42f;
        [field: Tooltip("Hop apex in units, per one unit of sphere radius.")]
        [field: SerializeField, Min(0f)] public float HopHeightRatio { get; private set; } = .6f;
        [field: Tooltip("How far the sphere stretches along the jump on take off.")]
        [field: SerializeField, Range(0f, 1f)] public float HopStretch { get; private set; } = .22f;
        [field: Tooltip("How far the sphere squashes on landing.")]
        [field: SerializeField, Range(0f, 1f)] public float HopSquash { get; private set; } = .28f;
        [field: Tooltip("Seconds the sphere takes to round itself back after the last hop.")]
        [field: SerializeField, Min(0f)] public float HopSettleTime { get; private set; } = .18f;

        [field: Header("Win")]
        [field: Tooltip("Distance in units the sphere rolls past the door before it disappears inside.")]
        [field: SerializeField, Min(0f)] public float DoorEnterDistance { get; private set; } = 7f;
        [field: Tooltip("Seconds the sphere takes to go through the doorway.")]
        [field: SerializeField, Min(0f)] public float DoorEnterTime { get; private set; } = .7f;

        [field: Header("Lose")]
        [field: Tooltip("Distance in units the sphere bounces back off the trees it cannot pass.")]
        [field: SerializeField, Min(0f)] public float BumpDistance { get; private set; } = 2.5f;
        [field: Tooltip("Seconds the bounce back takes.")]
        [field: SerializeField, Min(0f)] public float BumpTime { get; private set; } = .28f;
        [field: Tooltip("How far the sphere squashes against the trees.")]
        [field: SerializeField, Range(0f, 1f)] public float BumpSquash { get; private set; } = .35f;

        public Vector3 DoorPosition(Vector3 direction)
        {
            return direction * DoorDistance;
        }
    }
}

using UnityEngine;
namespace UtinComputer.Finish
{
    public readonly struct AdvanceInfo
    {
        public AdvanceInfo(Vector3 from, Vector3 to, float distance, FinishOutcome outcome)
        {
            From = from;
            To = to;
            Distance = distance;
            Outcome = outcome;
        }

        public Vector3 From { get; }
        public Vector3 To { get; }
        public float Distance { get; }
        public FinishOutcome Outcome { get; }
    }
}

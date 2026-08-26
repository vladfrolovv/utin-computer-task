using System.Collections.Generic;
using UnityEngine;
using UtinComputer.Utils;
namespace UtinComputer.Core
{
    public class DestroyablesController
    {
        private readonly List<IDestroyable> _destroyables = new();
        private readonly List<IDestroyable> _overlapped = new();

        public IReadOnlyList<IDestroyable> Destroyables => _destroyables;

        public void Register(IDestroyable destroyable)
        {
            _destroyables.Add(destroyable);
        }

        public void Unregister(IDestroyable destroyable)
        {
            _destroyables.Remove(destroyable);
        }

        public bool TryGetContactDistance(Vector3 origin, Vector3 direction, float radius, out float distance)
        {
            distance = float.PositiveInfinity;

            bool found = false;

            foreach (IDestroyable destroyable in _destroyables)
            {
                Vector3 delta = (destroyable.Position - origin).Flat();
                float forward = Vector3.Dot(delta, direction);

                if (forward <= 0f)
                    continue;

                float reach = radius + destroyable.Radius;
                float side = (delta - direction * forward).magnitude;

                if (side > reach)
                    continue;

                float contact = forward - Mathf.Sqrt(Mathf.Max(reach * reach - side * side, 0f));

                if (contact >= distance)
                    continue;

                distance = contact;
                found = true;
            }

            return found;
        }

        public void DestroyOverlapped(BlastInfo blast)
        {
            _overlapped.Clear();

            foreach (IDestroyable destroyable in _destroyables)
            {
                if (IsInRange(destroyable, blast.Origin, blast.Radius))
                    _overlapped.Add(destroyable);
            }

            foreach (IDestroyable destroyable in _overlapped)
                destroyable.Destroy(blast.Origin, blast.DelayAt(destroyable.Position));

            _overlapped.Clear();
        }

        private static bool IsInRange(IDestroyable destroyable, Vector3 center, float radius)
        {
            float reach = radius + destroyable.Radius;

            return (destroyable.Position.Flat() - center.Flat()).sqrMagnitude <= reach * reach;
        }
    }
}

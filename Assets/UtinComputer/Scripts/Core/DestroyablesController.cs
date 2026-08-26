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

        public bool Overlaps(Vector3 center, float radius)
        {
            foreach (IDestroyable destroyable in _destroyables)
            {
                if (IsOverlapped(destroyable, center, radius))
                    return true;
            }

            return false;
        }

        public void DestroyOverlapped(BlastInfo blast)
        {
            _overlapped.Clear();

            foreach (IDestroyable destroyable in _destroyables)
            {
                if (IsOverlapped(destroyable, blast.Origin, blast.Radius))
                    _overlapped.Add(destroyable);
            }

            foreach (IDestroyable destroyable in _overlapped)
                destroyable.Destroy(blast.Origin, blast.DelayAt(destroyable.Position));

            _overlapped.Clear();
        }

        private static bool IsOverlapped(IDestroyable destroyable, Vector3 center, float radius)
        {
            float reach = radius + destroyable.Radius;

            return (destroyable.Position.Flat() - center.Flat()).sqrMagnitude <= reach * reach;
        }
    }
}

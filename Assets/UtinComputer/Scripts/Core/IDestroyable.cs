using UnityEngine;
namespace UtinComputer.Core
{
    public interface IDestroyable
    {
        Vector3 Position { get; }
        float Radius { get; }

        void Destroy(Vector3 origin, float delay);
    }
}

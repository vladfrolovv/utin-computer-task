using UnityEngine;
namespace UtinComputer.Map
{
    public class TreeView : MonoBehaviour
    {
        [SerializeField] private Transform body;

        public void Apply(TreeInfo info)
        {
            transform.localPosition = info.Position;

            body.localRotation = Quaternion.Euler(0f, info.Rotation, 0f);
            body.localScale = Vector3.one * info.BodyScale;
        }
    }
}

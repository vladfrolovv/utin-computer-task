using Zenject;
namespace UtinComputer.Map
{
    public class TreeViewPool : MonoMemoryPool<TreeView>
    {
        protected override void OnDespawned(TreeView item)
        {
            item.Release();

            base.OnDespawned(item);
        }
    }
}

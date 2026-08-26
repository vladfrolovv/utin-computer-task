using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;
namespace UtinComputer.Map
{
    public class MapView : MonoBehaviour
    {
        private readonly List<TreeView> _spawned = new();
        private readonly List<TreeViewPool> _spawnedPools = new();

        private MapGenerationController _map;
        private List<TreeViewPool> _pools;

        [Inject]
        public void Construct(MapGenerationController map, List<TreeViewPool> pools)
        {
            _map = map;
            _pools = pools;
        }

        private void Start()
        {
            _map.Generated.Subscribe(Rebuild).AddTo(this);

            Rebuild(_map.Trees);
        }

        private void Rebuild(IReadOnlyList<TreeInfo> trees)
        {
            Clear();

            foreach (TreeInfo info in trees)
            {
                TreeViewPool pool = _pools[PoolIndex(info.Variant)];
                TreeView tree = pool.Spawn();

                tree.Apply(info);

                _spawned.Add(tree);
                _spawnedPools.Add(pool);
            }
        }

        private void Clear()
        {
            for (int i = 0; i < _spawned.Count; i++)
                _spawnedPools[i].Despawn(_spawned[i]);

            _spawned.Clear();
            _spawnedPools.Clear();
        }

        private int PoolIndex(float variant)
        {
            return Mathf.Clamp((int)(variant * _pools.Count), 0, _pools.Count - 1);
        }
    }
}

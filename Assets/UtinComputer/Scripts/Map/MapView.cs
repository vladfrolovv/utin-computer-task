using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;
namespace UtinComputer.Map
{
    public class MapView : MonoBehaviour
    {
        private readonly Dictionary<TreeView, TreeViewPool> _spawnedPools = new();
        private readonly Dictionary<TreeView, IDisposable> _spawnedSubscriptions = new();
        private readonly List<TreeView> _spawned = new();

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
                _spawnedPools[tree] = pool;
                _spawnedSubscriptions[tree] = tree.Destroyed.Subscribe(Despawn);
            }
        }

        private void Despawn(TreeView tree)
        {
            if (!_spawnedPools.TryGetValue(tree, out TreeViewPool pool))
                return;

            _spawnedSubscriptions[tree].Dispose();
            _spawnedSubscriptions.Remove(tree);
            _spawnedPools.Remove(tree);
            _spawned.Remove(tree);

            pool.Despawn(tree);
        }

        private void Clear()
        {
            foreach (TreeView tree in _spawned)
            {
                _spawnedSubscriptions[tree].Dispose();
                _spawnedPools[tree].Despawn(tree);
            }

            _spawned.Clear();
            _spawnedPools.Clear();
            _spawnedSubscriptions.Clear();
        }

        private int PoolIndex(float variant)
        {
            return Mathf.Clamp((int)(variant * _pools.Count), 0, _pools.Count - 1);
        }
    }
}

using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UtinComputer.Finish;
using UtinComputer.Spheres;
using UtinComputer.Utils;
using Zenject;
using Random = System.Random;
namespace UtinComputer.Map
{
    public class MapGenerationController : IInitializable
    {
        private readonly MapConfig _config;
        private readonly FinishConfig _finishConfig;
        private readonly SphereConfig _sphereConfig;
        private readonly List<TreeInfo> _trees = new();
        private readonly Subject<IReadOnlyList<TreeInfo>> _generated = new();

        private bool _isGenerated;

        public MapGenerationController(MapConfig config, FinishConfig finishConfig, SphereConfig sphereConfig)
        {
            _config = config;
            _finishConfig = finishConfig;
            _sphereConfig = sphereConfig;
        }

        public IObservable<IReadOnlyList<TreeInfo>> Generated => _generated;

        public IReadOnlyList<TreeInfo> Trees
        {
            get
            {
                EnsureGenerated();
                return _trees;
            }
        }

        public void Initialize()
        {
            EnsureGenerated();
        }

        public void Generate(int seed)
        {
            _isGenerated = true;
            Build(seed);
            _generated.OnNext(_trees);
        }

        private void EnsureGenerated()
        {
            if (_isGenerated)
                return;

            _isGenerated = true;
            Build(_config.Seed);
        }

        private void Build(int seed)
        {
            Random random = new(seed);

            _trees.Clear();

            Vector2 noiseOrigin = random.NextOrigin();
            Vector3 door = _finishConfig.DoorPosition(_sphereConfig.Direction);
            Vector2 doorFlat = new(door.x, door.z);
            int slots = Mathf.RoundToInt(_config.MapRadius / _config.TreeSpacing);
            float patchScale = _config.TreeSpacing / _config.ForestPatchSize;
            float densityThreshold = 1f - _config.Density;

            for (int x = -slots; x <= slots; x++)
            {
                for (int z = -slots; z <= slots; z++)
                {
                    float offsetX = random.NextRange(-_config.SpacingRandomness, _config.SpacingRandomness);
                    float offsetZ = random.NextRange(-_config.SpacingRandomness, _config.SpacingRandomness);
                    float rotation = random.NextRange(0f, 360f);
                    float scale = random.NextRange(_config.MinTreeScale, _config.MaxTreeScale);
                    float variant = random.NextNormalized();

                    Vector3 position = new(x * _config.TreeSpacing + offsetX, 0f, z * _config.TreeSpacing + offsetZ);

                    Vector2 flat = new(position.x, position.z);

                    if (flat.magnitude < _config.ClearingRadius)
                        continue;

                    if ((flat - doorFlat).magnitude < _finishConfig.DoorClearingRadius)
                        continue;

                    if (noiseOrigin.Sample(x, z, patchScale) < densityThreshold)
                        continue;

                    _trees.Add(new TreeInfo(position, scale, rotation, variant));
                }
            }
        }
    }
}

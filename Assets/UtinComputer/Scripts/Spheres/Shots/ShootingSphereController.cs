using System;
using UniRx;
using UnityEngine;
using UtinComputer.Core;
using UtinComputer.Spheres.Charges;
using UtinComputer.Utils;
using Zenject;
namespace UtinComputer.Spheres.Shots
{
    public class ShootingSphereController : ITickable
    {
        private readonly SphereConfig _config;
        private readonly SphereChargeController _sphereCharge;
        private readonly DestroyablesController _destroyables;

        private readonly ReactiveProperty<Vector3> _position = new();
        private readonly ReactiveProperty<float> _radius = new();
        private readonly ReactiveProperty<bool> _isFlying = new();

        private readonly Subject<Vector3> _launched = new();
        private readonly Subject<BlastInfo> _exploded = new();

        private Vector3 _origin;
        private Vector3 _direction;

        public ShootingSphereController(SphereConfig config, SphereChargeController sphereCharge,
            DestroyablesController destroyables)
        {
            _config = config;
            _sphereCharge = sphereCharge;
            _destroyables = destroyables;
        }

        public IReadOnlyReactiveProperty<Vector3> Position => _position;
        public IReadOnlyReactiveProperty<float> Radius => _radius;
        public IReadOnlyReactiveProperty<bool> IsFlying => _isFlying;

        public IObservable<Vector3> Launched => _launched;
        public IObservable<BlastInfo> Exploded => _exploded;

        public float BlastRadius => _radius.Value * _config.BlastRadiusPerShotRadius;

        public void Launch(Vector3 origin, float radius)
        {
            if (_isFlying.Value || radius <= 0f)
                return;

            _origin = origin;
            _direction = _sphereCharge.ShootDirection;
            _position.Value = origin;
            _radius.Value = radius;
            _isFlying.Value = true;

            _sphereCharge.SetBlocked(true);
            _launched.OnNext(origin);
        }

        public void Tick()
        {
            Step(Time.deltaTime);
        }

        public void Step(float deltaTime)
        {
            if (!_isFlying.Value || deltaTime <= 0f)
                return;

            _position.Value += _direction * (_config.ShotSpeed * deltaTime);

            if (_destroyables.Overlaps(_position.Value, _radius.Value))
            {
                Explode();
                return;
            }

            if ((_position.Value - _origin).Flat().magnitude >= _config.ShotMaxDistance)
                Explode();
        }

        private void Explode()
        {
            float blastRadius = BlastRadius;
            BlastInfo blast = new(_position.Value, blastRadius, blastRadius / _config.BlastWaveSpeed);

            _isFlying.Value = false;
            _destroyables.DestroyOverlapped(blast);

            _sphereCharge.SetBlocked(false);
            _exploded.OnNext(blast);
        }
    }
}

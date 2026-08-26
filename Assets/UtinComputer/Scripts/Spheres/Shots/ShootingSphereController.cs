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
        private readonly Subject<Unit> _settled = new();

        private Vector3 _origin;
        private Vector3 _target;
        private Vector3 _launchVelocity;
        private float _gravity;
        private float _flightTime;
        private float _elapsed;
        private float _cooldown;
        private bool _settling;

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
        public IObservable<Unit> Settled => _settled;

        public Vector3 Velocity => _launchVelocity + Vector3.down * (_gravity * _elapsed);
        public float BlastRadius => _radius.Value * _config.BlastRadiusPerShotRadius;

        public void Launch(Vector3 origin, float radius)
        {
            if (_isFlying.Value || radius <= 0f)
                return;

            _origin = origin;
            _radius.Value = radius;
            _target = Target(origin, radius);
            _flightTime = FlightTime(origin, _target);
            _gravity = 8f * _config.ShotArcHeight / (_flightTime * _flightTime);
            _launchVelocity = LaunchVelocity(origin, _target, _flightTime);
            _elapsed = 0f;
            _position.Value = origin;
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
            if (deltaTime <= 0f)
                return;

            if (!_isFlying.Value)
            {
                StepCooldown(deltaTime);
                return;
            }

            _elapsed += deltaTime;

            if (_elapsed >= _flightTime)
            {
                _position.Value = _target;
                Explode();
                return;
            }

            _position.Value = _origin + _launchVelocity * _elapsed
                              + Vector3.down * (.5f * _gravity * _elapsed * _elapsed);
        }

        private void StepCooldown(float deltaTime)
        {
            if (!_settling)
                return;

            _cooldown -= deltaTime;

            if (_cooldown > 0f)
                return;

            _settling = false;

            _sphereCharge.SetBlocked(false);
            _settled.OnNext(Unit.Default);
        }

        private Vector3 Target(Vector3 origin, float radius)
        {
            Vector3 direction = _sphereCharge.ShootDirection.Flat().normalized;

            if (!_destroyables.TryGetContactDistance(origin, direction, radius, out float distance))
                distance = _config.ShotMaxDistance;

            return origin.Flat() + direction * Mathf.Max(distance, radius) + Vector3.up * (_config.ShotGroundHeight + radius);
        }

        private float FlightTime(Vector3 origin, Vector3 target)
        {
            float distance = (target - origin).Flat().magnitude;

            return Mathf.Max(distance / _config.ShotSpeed, _config.ShotMinFlightTime);
        }

        private Vector3 LaunchVelocity(Vector3 origin, Vector3 target, float flightTime)
        {
            return (target - origin) / flightTime + Vector3.up * (.5f * _gravity * flightTime);
        }

        private void Explode()
        {
            float blastRadius = BlastRadius;
            BlastInfo blast = new(_position.Value, blastRadius, blastRadius / _config.BlastWaveSpeed);

            _isFlying.Value = false;
            _cooldown = _config.PostBlastDelay;
            _settling = true;

            _destroyables.DestroyOverlapped(blast);
            _exploded.OnNext(blast);
        }
    }
}

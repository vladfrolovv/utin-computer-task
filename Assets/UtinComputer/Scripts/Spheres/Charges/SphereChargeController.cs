using System;
using UniRx;
using UnityEngine;
using UtinComputer.Utils;
using Zenject;
namespace UtinComputer.Spheres.Charges
{
    public class SphereChargeController : IInitializable, ITickable
    {
        private readonly SphereConfig _config;

        private readonly ReactiveProperty<float> _radius = new();
        private readonly ReactiveProperty<float> _shotRadius = new();
        private readonly ReactiveProperty<float> _shotGrowthSpeed = new();
        private readonly ReactiveProperty<bool> _isCharging = new();
        private readonly ReactiveProperty<bool> _isLost = new();
        private readonly ReactiveProperty<bool> _isBlocked = new();
        private readonly ReactiveProperty<bool> _isRunning = new();

        private readonly Subject<Unit> _chargeStarted = new();
        private readonly Subject<float> _chargeReleased = new();

        private float _startVolume;
        private float _chargedVolume;
        private float _chargeTime;
        private bool _started;

        public SphereChargeController(SphereConfig config)
        {
            _config = config;
        }

        public IReadOnlyReactiveProperty<float> Radius => _radius;
        public IReadOnlyReactiveProperty<float> ShotRadius => _shotRadius;
        public IReadOnlyReactiveProperty<float> ShotGrowthSpeed => _shotGrowthSpeed;
        public IReadOnlyReactiveProperty<bool> IsCharging => _isCharging;
        public IReadOnlyReactiveProperty<bool> IsLost => _isLost;
        public IReadOnlyReactiveProperty<bool> IsBlocked => _isBlocked;
        public IReadOnlyReactiveProperty<bool> IsRunning => _isRunning;

        public IObservable<Unit> ChargeStarted => _chargeStarted;
        public IObservable<float> ChargeReleased => _chargeReleased;

        public Vector3 ShootDirection => _config.Direction;

        public bool HasReserve => _radius.Value > _config.TravelReserveRadius;

        public Vector3 ShotOffset => ShootDirection * (_radius.Value + _shotRadius.Value);

        public float ChargeProgress => Mathf.InverseLerp(_config.StartRadius, _config.MinRadius, _radius.Value);

        public void Initialize()
        {
            EnsureStarted();
        }

        public void BeginCharge()
        {
            EnsureStarted();

            if (!_isRunning.Value || _isLost.Value || _isBlocked.Value || _isCharging.Value || !HasReserve)
                return;

            _chargedVolume = _radius.Value.ToSphereVolume();
            _shotRadius.Value = 0f;
            _shotGrowthSpeed.Value = 0f;
            _chargeTime = 0f;
            _isCharging.Value = true;

            _chargeStarted.OnNext(Unit.Default);
        }

        public void SetBlocked(bool blocked)
        {
            _isBlocked.Value = blocked;
        }

        public void SetRunning(bool running)
        {
            _isRunning.Value = running;

            if (!running)
                EndCharge();
        }

        public void EndCharge()
        {
            if (!_isCharging.Value)
                return;

            float shotRadius = _shotRadius.Value;

            _isCharging.Value = false;
            _shotGrowthSpeed.Value = 0f;
            _shotRadius.Value = 0f;

            _chargeReleased.OnNext(shotRadius);
        }

        public void Tick()
        {
            Step(Time.deltaTime);
        }

        public void Step(float deltaTime)
        {
            EnsureStarted();

            if (!_isCharging.Value || deltaTime <= 0f)
                return;

            _chargeTime += deltaTime;

            float previousShotRadius = _shotRadius.Value;
            float shotVolume = previousShotRadius.ToSphereVolume() + ChargeVolumeSpeed() * deltaTime;

            _shotRadius.Value = shotVolume.ToSphereRadius();
            _radius.Value = (_chargedVolume - shotVolume).ToSphereRadius();
            _shotGrowthSpeed.Value = (_shotRadius.Value - previousShotRadius) / deltaTime;

            if (_radius.Value > _config.MinRadius)
                return;

            _isLost.Value = true;

            EndCharge();
        }

        private float ChargeVolumeSpeed()
        {
            float ramp = _config.ChargeAccelerationTime > 0f
                ? Mathf.SmoothStep(0f, 1f, _chargeTime / _config.ChargeAccelerationTime)
                : 1f;

            return _startVolume * _config.ChargeRatePerSecond * ramp;
        }

        private void EnsureStarted()
        {
            if (_started)
                return;

            _started = true;
            _startVolume = _config.StartRadius.ToSphereVolume();
            _radius.Value = _config.StartRadius;
            _shotRadius.Value = 0f;
            _chargedVolume = _startVolume;
        }
    }
}

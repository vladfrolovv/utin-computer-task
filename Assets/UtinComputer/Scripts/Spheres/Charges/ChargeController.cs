using UniRx;
using UnityEngine;
using UtinComputer.Utils;
using Zenject;
namespace UtinComputer.Spheres.Charges
{
    public class ChargeController : IInitializable, ITickable
    {
        private readonly SphereConfig _config;

        private readonly ReactiveProperty<float> _radius = new();
        private readonly ReactiveProperty<float> _shotRadius = new();
        private readonly ReactiveProperty<bool> _isCharging = new();
        private readonly ReactiveProperty<bool> _isLost = new();

        private float _chargedVolume;

        public ChargeController(SphereConfig config)
        {
            _config = config;
        }

        public IReadOnlyReactiveProperty<float> Radius => _radius;
        public IReadOnlyReactiveProperty<float> ShotRadius => _shotRadius;
        public IReadOnlyReactiveProperty<bool> IsCharging => _isCharging;
        public IReadOnlyReactiveProperty<bool> IsLost => _isLost;

        public void Initialize()
        {
            _radius.Value = _config.StartRadius;
            _shotRadius.Value = 0f;
            _chargedVolume = _config.StartRadius.ToSphereVolume();
        }

        public void BeginCharge()
        {
            if (_isLost.Value)
                return;

            _chargedVolume = _radius.Value.ToSphereVolume();
            _isCharging.Value = true;
        }

        public void EndCharge()
        {
            if (!_isCharging.Value)
                return;

            _isCharging.Value = false;
            _shotRadius.Value = 0f;
        }

        public void Tick()
        {
            Step(Time.deltaTime);
        }

        public void Step(float deltaTime)
        {
            if (!_isCharging.Value)
                return;

            float shotVolume = _shotRadius.Value.ToSphereVolume() + _config.ChargeVolumePerSecond * deltaTime;

            _shotRadius.Value = shotVolume.ToSphereRadius();
            _radius.Value = (_chargedVolume - shotVolume).ToSphereRadius();

            if (_radius.Value > _config.MinRadius)
                return;

            _isCharging.Value = false;
            _isLost.Value = true;
        }
    }
}

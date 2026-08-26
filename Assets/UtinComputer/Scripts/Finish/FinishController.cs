using System;
using UniRx;
using UnityEngine;
using UtinComputer.Core;
using UtinComputer.Spheres;
using UtinComputer.Spheres.Charges;
using UtinComputer.Spheres.Shots;
using Zenject;
namespace UtinComputer.Finish
{
    public class FinishController : IInitializable, IDisposable
    {
        private readonly FinishConfig _config;
        private readonly SphereConfig _sphereConfig;
        private readonly SphereChargeController _sphereCharge;
        private readonly ShootingSphereController _shootingSphere;
        private readonly DestroyablesController _destroyables;
        private readonly CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<float> _traveled = new();
        private readonly ReactiveProperty<FinishOutcome> _outcome = new();
        private readonly Subject<AdvanceInfo> _advanced = new();
        private readonly Subject<FinishOutcome> _finished = new();

        public FinishController(FinishConfig config, SphereConfig sphereConfig, SphereChargeController sphereCharge,
            ShootingSphereController shootingSphere, DestroyablesController destroyables)
        {
            _config = config;
            _sphereConfig = sphereConfig;
            _sphereCharge = sphereCharge;
            _shootingSphere = shootingSphere;
            _destroyables = destroyables;
        }

        public IReadOnlyReactiveProperty<float> Traveled => _traveled;
        public IReadOnlyReactiveProperty<FinishOutcome> Outcome => _outcome;
        public IObservable<AdvanceInfo> Advanced => _advanced;
        public IObservable<FinishOutcome> Finished => _finished;

        public Vector3 Direction => _sphereConfig.Direction;
        public Vector3 DoorPosition => _config.DoorPosition(Direction);
        public Vector3 Position => Direction * _traveled.Value;
        public float DistanceToDoor => Mathf.Max(_config.DoorDistance - _traveled.Value, 0f);

        public void Initialize()
        {
            _shootingSphere.Settled.Subscribe(OnSettled).AddTo(_compositeDisposable);
            _sphereCharge.IsLost.Where(IsDrained).Subscribe(OnDrained).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }

        public void ReportAdvanceCompleted()
        {
            if (_outcome.Value != FinishOutcome.None)
                return;

            _sphereCharge.SetBlocked(false);
        }

        public void ReportFinished()
        {
            _finished.OnNext(_outcome.Value);
        }

        private void OnSettled(Unit unit)
        {
            if (_outcome.Value != FinishOutcome.None)
                return;

            float distanceToDoor = DistanceToDoor;
            float step = Mathf.Min(Frontier(), distanceToDoor);

            if (step >= distanceToDoor)
                Resolve(distanceToDoor, FinishOutcome.Win);
            else if (_sphereCharge.Radius.Value <= _sphereConfig.TravelReserveRadius)
                Resolve(step, FinishOutcome.Lose);
            else if (step >= _config.MinAdvanceDistance)
                Resolve(step, FinishOutcome.None);
        }

        private void OnDrained(bool lost)
        {
            if (_outcome.Value != FinishOutcome.None)
                return;

            Resolve(0f, FinishOutcome.Lose);
        }

        private void Resolve(float step, FinishOutcome outcome)
        {
            Vector3 from = Position;

            _outcome.Value = outcome;
            _traveled.Value += Mathf.Max(step, 0f);
            _sphereCharge.SetBlocked(true);

            _advanced.OnNext(new AdvanceInfo(from, Position, Mathf.Max(step, 0f), outcome));
        }

        private float Frontier()
        {
            float radius = _sphereCharge.Radius.Value + _config.CorridorClearance;

            if (!_destroyables.TryGetContactDistance(Position, Direction, radius, out float distance))
                return float.PositiveInfinity;

            return Mathf.Max(distance - _config.BlockerGap, 0f);
        }

        private static bool IsDrained(bool lost)
        {
            return lost;
        }
    }
}

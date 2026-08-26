using System;
using UniRx;
using UnityEngine.SceneManagement;
using UtinComputer.Spheres.Charges;
using Zenject;
namespace UtinComputer.Finish
{
    public class GameFlowController : IInitializable, IDisposable
    {
        private readonly SphereChargeController _sphereCharge;
        private readonly FinishController _finish;
        private readonly CompositeDisposable _compositeDisposable = new();

        private readonly ReactiveProperty<bool> _isPlaying = new();

        public GameFlowController(SphereChargeController sphereCharge, FinishController finish)
        {
            _sphereCharge = sphereCharge;
            _finish = finish;
        }

        public IReadOnlyReactiveProperty<bool> IsPlaying => _isPlaying;

        public void Initialize()
        {
            _sphereCharge.SetRunning(false);
            _finish.Finished.Subscribe(OnFinished).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }

        public void Play()
        {
            if (_finish.Outcome.Value != FinishOutcome.None)
            {
                Restart();
                return;
            }

            if (_isPlaying.Value)
                return;

            _isPlaying.Value = true;
            _sphereCharge.SetRunning(true);
        }

        private void OnFinished(FinishOutcome outcome)
        {
            _isPlaying.Value = false;
            _sphereCharge.SetRunning(false);
        }

        private static void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

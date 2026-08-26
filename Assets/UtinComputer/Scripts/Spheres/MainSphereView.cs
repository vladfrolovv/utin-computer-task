using DG.Tweening;
using UniRx;
using UnityEngine;
using UtinComputer.Cameras;
using UtinComputer.Spheres.Charges;
using Zenject;
namespace UtinComputer.Spheres
{
    public class MainSphereView : MonoBehaviour
    {
        [SerializeField] private Transform body;

        private SphereConfig _config;
        private SphereChargeController _sphereCharge;
        private CameraFollowController _cameraFollow;
        private Vector3 _origin;
        private Tween _returnTween;
        private Tween _punchTween;

        [Inject]
        public void Construct(SphereConfig config, SphereChargeController sphereCharge, CameraFollowController cameraFollow)
        {
            _config = config;
            _sphereCharge = sphereCharge;
            _cameraFollow = cameraFollow;
        }

        public Vector3 Center => body.position;

        private void Start()
        {
            _origin = body.localPosition;

            _cameraFollow.SetTarget(transform);

            _sphereCharge.Radius.Subscribe(OnRadius).AddTo(this);
            _sphereCharge.ChargeStarted.Subscribe(OnChargeStarted).AddTo(this);
            _sphereCharge.ChargeReleased.Subscribe(OnChargeReleased).AddTo(this);
            _sphereCharge.IsLost.Subscribe(OnLost).AddTo(this);

            Observable.EveryUpdate()
                .Where(IsCharging)
                .Subscribe(OnChargeUpdate)
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _returnTween?.Kill();
            _punchTween?.Kill();
        }

        private void OnRadius(float radius)
        {
            if (_punchTween != null && _punchTween.IsActive() && _punchTween.IsPlaying())
                return;

            body.localScale = Vector3.one * (radius * 2f);
        }

        private void OnChargeStarted(Unit unit)
        {
            _returnTween?.Kill();
            _punchTween?.Kill();

            body.localScale = Vector3.one * (_sphereCharge.Radius.Value * 2f);
        }

        private void OnChargeUpdate(long frame)
        {
            body.localPosition = _origin + Shake() - _sphereCharge.ShootDirection * Recoil();
        }

        private void OnChargeReleased(float shotRadius)
        {
            _returnTween?.Kill();
            _returnTween = body.DOLocalMove(_origin, _config.RecoilReturnTime).SetEase(Ease.OutBack);

            if (_sphereCharge.IsLost.Value)
                return;

            _punchTween?.Kill();
            _punchTween = body.DOPunchScale(Vector3.one * (_sphereCharge.Radius.Value * 2f * _config.ReleasePunchRatio),
                _config.ReleasePunchTime, 6, .8f);
        }

        private void OnLost(bool lost)
        {
            if (!lost)
                return;

            _returnTween?.Kill();
            _punchTween?.Kill();
            body.DOScale(0f, _config.LoseCollapseTime).SetEase(Ease.InBack);
        }

        private Vector3 Shake()
        {
            float time = Time.time * _config.ShakeFrequency;
            float amplitude = _config.ShakeAmplitudeRatio * _sphereCharge.Radius.Value * ShakeRamp();
            Vector3 noise = new(Mathf.PerlinNoise(time, 0f) - .5f, Mathf.PerlinNoise(0f, time) - .5f, 0f);

            return noise * (amplitude * 2f);
        }

        private float ShakeRamp()
        {
            return Mathf.Pow(_sphereCharge.ChargeProgress, _config.ShakeRampPower);
        }

        private float Recoil()
        {
            return _config.RecoilRatio * _sphereCharge.Radius.Value * _sphereCharge.ChargeProgress;
        }

        private bool IsCharging(long frame)
        {
            return _sphereCharge.IsCharging.Value;
        }
    }
}

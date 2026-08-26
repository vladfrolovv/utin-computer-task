using DG.Tweening;
using UniRx;
using UnityEngine;
using UtinComputer.Cameras;
using UtinComputer.Core;
using UtinComputer.Effects;
using UtinComputer.Spheres.Charges;
using UtinComputer.Spheres.Shots;
using Zenject;
namespace UtinComputer.Spheres
{
    public class ShootingSphereView : MonoBehaviour
    {
        [SerializeField] private Transform body;
        [SerializeField] private BlastWaveView blastWave;

        private SphereConfig _config;
        private CameraConfig _cameraConfig;
        private SphereChargeController _sphereCharge;
        private ShootingSphereController _shootingSphere;
        private MainSphereView _mainSphere;
        private CameraFollowController _cameraFollow;

        private Tween _emergeTween;
        private Sequence _explodeSequence;
        private float _emerge;
        private float _stretch;

        [Inject]
        public void Construct(SphereConfig config, CameraConfig cameraConfig, SphereChargeController sphereCharge,
            ShootingSphereController shootingSphere, MainSphereView mainSphere, CameraFollowController cameraFollow)
        {
            _config = config;
            _cameraConfig = cameraConfig;
            _sphereCharge = sphereCharge;
            _shootingSphere = shootingSphere;
            _mainSphere = mainSphere;
            _cameraFollow = cameraFollow;
        }

        private void Start()
        {
            body.gameObject.SetActive(false);

            _sphereCharge.ChargeStarted.Subscribe(OnChargeStarted).AddTo(this);
            _sphereCharge.ChargeReleased.Subscribe(OnChargeReleased).AddTo(this);

            _shootingSphere.Launched.Subscribe(OnLaunched).AddTo(this);
            _shootingSphere.Position.Subscribe(OnPosition).AddTo(this);
            _shootingSphere.Exploded.Subscribe(OnExploded).AddTo(this);

            Observable.EveryUpdate()
                .Where(IsCharging)
                .Subscribe(OnChargeUpdate)
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _emergeTween?.Kill();
            _explodeSequence?.Kill();
        }

        private void OnChargeStarted(Unit unit)
        {
            _emergeTween?.Kill();
            _explodeSequence?.Kill();

            _emerge = 0f;
            _stretch = 0f;

            body.gameObject.SetActive(true);
            ApplyCharge();

            _emergeTween = DOVirtual.Float(0f, 1f, _config.EmergeTime, OnEmerge).SetEase(Ease.OutBack);
        }

        private void OnEmerge(float emerge)
        {
            _emerge = emerge;
            ApplyCharge();
        }

        private void OnChargeUpdate(long frame)
        {
            float target = Mathf.Min(_sphereCharge.ShotGrowthSpeed.Value * _config.StretchPerGrowthSpeed, _config.MaxStretch);

            _stretch = Mathf.Lerp(_stretch, target, 1f - Mathf.Exp(-_config.StretchSmoothing * Time.deltaTime));

            ApplyCharge();
        }

        private void OnChargeReleased(float shotRadius)
        {
            _emergeTween?.Kill();

            _shootingSphere.Launch(body.position, shotRadius);

            if (!_shootingSphere.IsFlying.Value)
                Hide();
        }

        private void OnLaunched(Vector3 origin)
        {
            _explodeSequence?.Kill();

            body.gameObject.SetActive(true);
            body.position = origin;
            body.rotation = Quaternion.LookRotation(_sphereCharge.ShootDirection);
            body.localScale = Scale(_shootingSphere.Radius.Value * 2f, _stretch);

            _cameraFollow.SetTarget(body, _cameraConfig.ShotFollowSmoothing);
        }

        private void OnPosition(Vector3 position)
        {
            if (!_shootingSphere.IsFlying.Value)
                return;

            body.position = position;
            body.localScale = Scale(_shootingSphere.Radius.Value * 2f, _config.FlightStretch);
        }

        private void OnExploded(BlastInfo blast)
        {
            _cameraFollow.SetTarget(_mainSphere.transform);
            blastWave.Play(blast, _config.BlastWaveGroundHeight);

            _explodeSequence?.Kill();
            _explodeSequence = DOTween.Sequence()
                .Append(body.DOScale(Vector3.zero, _config.ReleaseVanishTime).SetEase(Ease.InBack))
                .OnComplete(Hide);
        }

        private void ApplyCharge()
        {
            float radius = _sphereCharge.ShotRadius.Value * _emerge;
            Vector3 direction = _sphereCharge.ShootDirection;

            body.rotation = Quaternion.LookRotation(direction);
            body.position = _mainSphere.Center + direction * (_sphereCharge.Radius.Value + radius);
            body.localScale = Scale(radius * 2f, _stretch);
        }

        private Vector3 Scale(float diameter, float stretch)
        {
            return new Vector3(1f - stretch * .5f, 1f - stretch * .5f, 1f + stretch) * diameter;
        }

        private void Hide()
        {
            body.gameObject.SetActive(false);
        }

        private bool IsCharging(long frame)
        {
            return _sphereCharge.IsCharging.Value;
        }
    }
}

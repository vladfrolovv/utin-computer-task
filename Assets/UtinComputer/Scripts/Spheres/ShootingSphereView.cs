using DG.Tweening;
using UniRx;
using UnityEngine;
using UtinComputer.Spheres.Charges;
using Zenject;
namespace UtinComputer.Spheres
{
    public class ShootingSphereView  : MonoBehaviour
    {
        [SerializeField] private Transform body;

        private SphereConfig _config;
        private SphereChargeController _sphereCharge;
        private MainSphereView _mainSphere;

        private Tween _emergeTween;
        private Sequence _releaseSequence;
        private float _emerge;
        private float _stretch;

        [Inject]
        public void Construct(SphereConfig config, SphereChargeController sphereCharge, MainSphereView mainSphere)
        {
            _config = config;
            _sphereCharge = sphereCharge;
            _mainSphere = mainSphere;
        }

        private void Start()
        {
            body.gameObject.SetActive(false);

            _sphereCharge.ChargeStarted.Subscribe(OnChargeStarted).AddTo(this);
            _sphereCharge.ChargeReleased.Subscribe(OnChargeReleased).AddTo(this);

            Observable.EveryUpdate()
                .Where(IsCharging)
                .Subscribe(OnChargeUpdate)
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _emergeTween?.Kill();
            _releaseSequence?.Kill();
        }

        private void OnChargeStarted(Unit unit)
        {
            _emergeTween?.Kill();
            _releaseSequence?.Kill();

            _emerge = 0f;
            _stretch = 0f;

            body.gameObject.SetActive(true);
            Apply();

            _emergeTween = DOVirtual.Float(0f, 1f, _config.EmergeTime, OnEmerge).SetEase(Ease.OutBack);
        }

        private void OnEmerge(float emerge)
        {
            _emerge = emerge;
            Apply();
        }

        private void OnChargeUpdate(long frame)
        {
            float target = Mathf.Min(_sphereCharge.ShotGrowthSpeed.Value * _config.StretchPerGrowthSpeed, _config.MaxStretch);

            _stretch = Mathf.Lerp(_stretch, target, 1f - Mathf.Exp(-_config.StretchSmoothing * Time.deltaTime));

            Apply();
        }

        private void OnChargeReleased(float shotRadius)
        {
            _emergeTween?.Kill();

            Vector3 direction = _sphereCharge.ShootDirection;
            float diameter = shotRadius * 2f;
            Vector3 kick = body.position + direction * (shotRadius * _config.ReleaseKickRatio);

            _releaseSequence?.Kill();
            _releaseSequence = DOTween.Sequence()
                .Append(body.DOMove(kick, _config.ReleaseRoundTime).SetEase(Ease.OutQuad))
                .Join(body.DOScale(Vector3.one * diameter, _config.ReleaseRoundTime).SetEase(Ease.OutBack))
                .Append(body.DOScale(Vector3.zero, _config.ReleaseVanishTime).SetEase(Ease.InBack))
                .OnComplete(Hide);
        }

        private void Apply()
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

using DG.Tweening;
using UniRx;
using UnityEngine;
using UtinComputer.Spheres.Charges;
using Zenject;
namespace UtinComputer.Spheres
{
    public class MainSphereView : MonoBehaviour
    {
        [SerializeField] private Transform body;

        private SphereConfig _config;
        private ChargeController _charge;
        private Vector3 _origin;

        [Inject]
        public void Construct(SphereConfig config, ChargeController charge)
        {
            _config = config;
            _charge = charge;
        }

        private void Start()
        {
            _origin = body.localPosition;

            _charge.Radius.Subscribe(OnRadius).AddTo(this);
            _charge.IsCharging.Subscribe(OnCharging).AddTo(this);
            _charge.IsLost.Subscribe(OnLost).AddTo(this);

            Observable.EveryUpdate()
                .Where(IsCharging)
                .Subscribe(OnChargeUpdate)
                .AddTo(this);
        }

        private void OnRadius(float radius)
        {
            body.localScale = Vector3.one * (radius * 2f);
        }

        private void OnCharging(bool charging)
        {
            if (charging)
                return;

            body.localPosition = _origin;
        }

        private void OnChargeUpdate(long frame)
        {
            body.localPosition = _origin + Shake();
        }

        private void OnLost(bool lost)
        {
            if (!lost)
                return;

            body.DOScale(0f, _config.LoseCollapseTime).SetEase(Ease.InBack);
        }

        private Vector3 Shake()
        {
            float time = Time.time * _config.ShakeFrequency;
            float amplitude = _config.ShakeAmplitude * Mathf.InverseLerp(_config.StartRadius, _config.MinRadius, _charge.Radius.Value);
            Vector3 noise = new(Mathf.PerlinNoise(time, 0f) - .5f, Mathf.PerlinNoise(0f, time) - .5f, 0f);

            return noise * (amplitude * 2f);
        }

        private bool IsCharging(long frame)
        {
            return _charge.IsCharging.Value;
        }
    }
}

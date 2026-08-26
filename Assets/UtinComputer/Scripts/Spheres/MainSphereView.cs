using DG.Tweening;
using UniRx;
using UnityEngine;
using UtinComputer.Cameras;
using UtinComputer.Finish;
using UtinComputer.Spheres.Charges;
using Zenject;
namespace UtinComputer.Spheres
{
    public class MainSphereView : MonoBehaviour
    {
        [SerializeField] private Transform body;
        [SerializeField] private Transform reserveMarker;

        private SphereConfig _config;
        private FinishConfig _finishConfig;
        private SphereChargeController _sphereCharge;
        private FinishController _finish;
        private CameraFollowController _cameraFollow;
        private Vector3 _origin;
        private Vector3 _startPosition;
        private Tween _returnTween;
        private Tween _punchTween;
        private Sequence _travelSequence;

        [Inject]
        public void Construct(SphereConfig config, FinishConfig finishConfig, SphereChargeController sphereCharge,
            FinishController finish, CameraFollowController cameraFollow)
        {
            _config = config;
            _finishConfig = finishConfig;
            _sphereCharge = sphereCharge;
            _finish = finish;
            _cameraFollow = cameraFollow;
        }

        public Vector3 Center => body.position;

        private void Start()
        {
            _origin = body.localPosition;
            _startPosition = transform.position;

            ApplyReserveMarker();

            _cameraFollow.SetTarget(transform);

            _sphereCharge.Radius.Subscribe(OnRadius).AddTo(this);
            _sphereCharge.ChargeStarted.Subscribe(OnChargeStarted).AddTo(this);
            _sphereCharge.ChargeReleased.Subscribe(OnChargeReleased).AddTo(this);

            _finish.Advanced.Subscribe(OnAdvanced).AddTo(this);

            Observable.EveryUpdate()
                .Where(IsCharging)
                .Subscribe(OnChargeUpdate)
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _returnTween?.Kill();
            _punchTween?.Kill();
            _travelSequence?.Kill();
        }

        private void OnRadius(float radius)
        {
            if (IsAnimating())
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

        private void OnAdvanced(AdvanceInfo info)
        {
            _returnTween?.Kill();
            _punchTween?.Kill();
            _travelSequence?.Kill();

            body.localPosition = _origin;
            body.localScale = Vector3.one * Diameter();

            _travelSequence = DOTween.Sequence();

            AppendHops(_travelSequence, info);
            AppendOutcome(_travelSequence, info);
        }

        private void AppendHops(Sequence sequence, AdvanceInfo info)
        {
            int hops = Mathf.CeilToInt(info.Distance / _finishConfig.HopDistance);

            if (hops <= 0)
                return;

            for (int hop = 1; hop <= hops; hop++)
            {
                Vector3 from = _startPosition + Vector3.Lerp(info.From, info.To, (hop - 1f) / hops);
                Vector3 to = _startPosition + Vector3.Lerp(info.From, info.To, (float)hop / hops);

                sequence.Append(Hop(from, to));
            }

            sequence.Append(body.DOScale(Vector3.one * Diameter(), _finishConfig.HopSettleTime).SetEase(Ease.OutBack));
        }

        private Sequence Hop(Vector3 from, Vector3 to)
        {
            float time = _finishConfig.HopTime;
            float height = _finishConfig.HopHeightRatio * _sphereCharge.Radius.Value;
            float diameter = Diameter();

            Sequence hop = DOTween.Sequence();

            hop.AppendCallback(() => transform.position = from);
            hop.Append(transform.DOMove(to, time).SetEase(Ease.Linear));
            hop.Join(body.DOLocalMoveY(_origin.y + height, time * .5f).SetEase(Ease.OutQuad));
            hop.Insert(time * .5f, body.DOLocalMoveY(_origin.y, time * .5f).SetEase(Ease.InQuad));
            hop.Insert(0f, body.DOScale(Stretch(_finishConfig.HopStretch) * diameter, time * .35f).SetEase(Ease.OutQuad));
            hop.Insert(time * .35f, body.DOScale(Vector3.one * diameter, time * .35f).SetEase(Ease.InOutQuad));
            hop.Insert(time * .8f, body.DOScale(Squash(_finishConfig.HopSquash) * diameter, time * .2f).SetEase(Ease.InQuad));

            return hop;
        }

        private void AppendOutcome(Sequence sequence, AdvanceInfo info)
        {
            switch (info.Outcome)
            {
                case FinishOutcome.Win:
                    AppendWin(sequence, info);
                    break;
                case FinishOutcome.Lose:
                    AppendLose(sequence);
                    break;
                default:
                    sequence.AppendCallback(_finish.ReportAdvanceCompleted);
                    return;
            }

            sequence.OnComplete(OnTravelFinished);
        }

        private void OnTravelFinished()
        {
            reserveMarker.gameObject.SetActive(false);

            _finish.ReportFinished();
        }

        private void AppendWin(Sequence sequence, AdvanceInfo info)
        {
            Vector3 through = _startPosition + info.To + _finish.Direction * _finishConfig.DoorEnterDistance;

            sequence.Append(transform.DOMove(through, _finishConfig.DoorEnterTime).SetEase(Ease.InQuad));
            sequence.Join(body.DOScale(Vector3.zero, _finishConfig.DoorEnterTime).SetEase(Ease.InBack));
        }

        private void AppendLose(Sequence sequence)
        {
            float diameter = Diameter();

            sequence.Append(body.DOScale(Squash(_finishConfig.BumpSquash) * diameter, _finishConfig.BumpTime)
                .SetEase(Ease.OutQuad));
            sequence.Join(transform.DOMove(-_finish.Direction * _finishConfig.BumpDistance, _finishConfig.BumpTime)
                .SetRelative(true).SetEase(Ease.OutQuad));
            sequence.Append(body.DOScale(Vector3.zero, _config.LoseCollapseTime).SetEase(Ease.InBack));
        }

        private void ApplyReserveMarker()
        {
            float diameter = _config.TravelReserveRadius * 2f;
            Vector3 scale = reserveMarker.localScale;

            reserveMarker.localScale = new Vector3(diameter, scale.y, diameter);
        }

        private float Diameter()
        {
            return _sphereCharge.Radius.Value * 2f;
        }

        private static Vector3 Stretch(float amount)
        {
            return new Vector3(1f - amount * .5f, 1f + amount, 1f - amount * .5f);
        }

        private static Vector3 Squash(float amount)
        {
            return new Vector3(1f + amount * .5f, 1f - amount, 1f + amount * .5f);
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

        private bool IsAnimating()
        {
            if (_travelSequence != null && _travelSequence.IsActive() && _travelSequence.IsPlaying())
                return true;

            return _punchTween != null && _punchTween.IsActive() && _punchTween.IsPlaying();
        }

        private bool IsCharging(long frame)
        {
            return _sphereCharge.IsCharging.Value;
        }
    }
}

using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UtinComputer.Core;
using UtinComputer.Utils;
using Zenject;
namespace UtinComputer.Map
{
    public class TreeView : MonoBehaviour, IDestroyable
    {
        [SerializeField] private Transform body;

        private readonly Subject<TreeView> _destroyed = new();

        private MapConfig _config;
        private DestroyablesController _destroyables;
        private Tween _fallTween;
        private Quaternion _standing;
        private float _bodyScale;

        [Inject]
        public void Construct(MapConfig config, DestroyablesController destroyables)
        {
            _config = config;
            _destroyables = destroyables;
        }

        public IObservable<TreeView> Destroyed => _destroyed;

        public Vector3 Position => transform.position;
        public float Radius => _bodyScale * _config.TreeRadiusPerScale;

        public void Apply(TreeInfo info)
        {
            _bodyScale = info.BodyScale;
            _standing = Quaternion.Euler(0f, info.Rotation, 0f);

            transform.localPosition = info.Position;

            body.localPosition = Vector3.zero;
            body.localRotation = _standing;
            body.localScale = Vector3.one * info.BodyScale;

            _destroyables.Register(this);
        }

        public void Release()
        {
            _destroyables.Unregister(this);

            _fallTween?.Kill();
            _fallTween = null;
        }

        void IDestroyable.Destroy(Vector3 origin, float delay)
        {
            _destroyables.Unregister(this);

            _fallTween?.Kill();
            _fallTween = Fall(FallAxis(origin), delay);
        }

        private void OnDestroy()
        {
            _fallTween?.Kill();
        }

        private Vector3 FallAxis(Vector3 origin)
        {
            Vector3 away = (transform.position - origin).Flat();

            if (away.sqrMagnitude < .0001f)
                away = body.forward.Flat();

            return Vector3.Cross(Vector3.up, away.normalized);
        }

        private Tween Fall(Vector3 axis, float delay)
        {
            Quaternion fallen = Quaternion.AngleAxis(_config.TreeFallAngle + _config.TreeSettleAngle, axis) * _standing;
            Quaternion settled = Quaternion.AngleAxis(_config.TreeFallAngle, axis) * _standing;

            return DOTween.Sequence()
                .AppendInterval(delay)
                .Append(body.DOLocalRotateQuaternion(fallen, _config.TreeFallTime).SetEase(Ease.InQuad))
                .Append(body.DOLocalRotateQuaternion(settled, _config.TreeSettleTime).SetEase(Ease.OutQuad))
                .AppendInterval(_config.TreeLieTime)
                .Append(body.DOLocalMoveY(-_config.TreeSinkDepth * _bodyScale, _config.TreeSinkTime).SetEase(Ease.InQuad))
                .OnComplete(OnCollapsed);
        }

        private void OnCollapsed()
        {
            _destroyed.OnNext(this);
        }
    }
}

using System;
using UniRx;
using UnityEngine;
namespace UtinComputer.Cameras
{
    public class CameraFollowController : IDisposable
    {
        private readonly CameraConfig _config;
        private readonly CameraRigView _rig;
        private readonly CompositeDisposable _compositeDisposable = new();

        private Transform _currentTarget;
        private Vector3 _targetDelta;
        private float _smoothing;

        public CameraFollowController(CameraConfig config, CameraRigView cameraRigView)
        {
            _config = config;
            _rig = cameraRigView;
            _smoothing = config.FollowSmoothing;

            Observable.EveryLateUpdate().Subscribe(Follow).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
            _compositeDisposable?.Clear();
        }

        public void SetTarget(Transform target)
        {
            SetTarget(target, _config.FollowSmoothing);
        }

        public void SetTarget(Transform target, float smoothing)
        {
            if (target == null)
                return;

            if (_currentTarget == null)
                _targetDelta = _rig.transform.position - target.position;

            _currentTarget = target;
            _smoothing = smoothing;
        }

        private void Follow(long frame)
        {
            if (_currentTarget == null)
                return;

            Vector3 desired = _currentTarget.position + _targetDelta;

            _rig.transform.position = Vector3.Lerp(_rig.transform.position, desired,
                1f - Mathf.Exp(-_smoothing * Time.deltaTime));
        }
    }
}

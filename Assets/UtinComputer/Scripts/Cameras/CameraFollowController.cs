using System;
using UniRx;
using UnityEngine;
namespace UtinComputer.Cameras
{
    public class CameraFollowController : IDisposable
    {
        private readonly CameraRigView _rig;
        private readonly CompositeDisposable _compositeDisposable = new();

        private Transform _currentTarget;
        private Vector3 _targetDela;

        public CameraFollowController(CameraRigView cameraRigView)
        {
            _rig = cameraRigView;
            Observable.EveryLateUpdate().Subscribe(Follow).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
            _compositeDisposable?.Clear();
        }

        public void SetTarget(Transform target)
        {
            _currentTarget = target;
            _targetDela = _rig.transform.position - target.position;
        }

        private void Follow(long l)
        {
            if (_currentTarget == null)
                return;

            _rig.transform.position = Vector3.Lerp(_rig.transform.position, _currentTarget.position + _targetDela, Time.deltaTime);
        }
    }
}

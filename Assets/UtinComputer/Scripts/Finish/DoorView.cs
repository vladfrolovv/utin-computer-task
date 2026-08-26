using DG.Tweening;
using UniRx;
using UnityEngine;
using UtinComputer.Spheres;
using UtinComputer.Utils;
using Zenject;
namespace UtinComputer.Finish
{
    public class DoorView : MonoBehaviour
    {
        [SerializeField] private Transform leftLeaf;
        [SerializeField] private Transform rightLeaf;

        private FinishConfig _config;
        private FinishController _finish;
        private MainSphereView _mainSphere;
        private Sequence _leavesSequence;

        [Inject]
        public void Construct(FinishConfig config, FinishController finish, MainSphereView mainSphere)
        {
            _config = config;
            _finish = finish;
            _mainSphere = mainSphere;
        }

        private void Start()
        {
            transform.position = _finish.DoorPosition;
            transform.rotation = Quaternion.LookRotation(_finish.Direction);

            leftLeaf.localRotation = Quaternion.identity;
            rightLeaf.localRotation = Quaternion.identity;

            Observable.EveryUpdate()
                .Where(IsSphereNear)
                .Take(1)
                .Subscribe(OnSphereNear)
                .AddTo(this);

            Observable.EveryUpdate()
                .Where(IsSphereThrough)
                .Take(1)
                .Subscribe(OnSphereThrough)
                .AddTo(this);
        }

        private void OnDestroy()
        {
            _leavesSequence?.Kill();
        }

        private void OnSphereNear(long frame)
        {
            Swing(-_config.DoorOpenAngle, _config.DoorOpenTime, Ease.OutBack);
        }

        private void OnSphereThrough(long frame)
        {
            Swing(0f, _config.DoorCloseTime, Ease.InQuad);
        }

        private void Swing(float angle, float duration, Ease ease)
        {
            _leavesSequence?.Kill();
            _leavesSequence = DOTween.Sequence()
                .Append(leftLeaf.DOLocalRotate(new Vector3(0f, angle, 0f), duration).SetEase(ease))
                .Join(rightLeaf.DOLocalRotate(new Vector3(0f, -angle, 0f), duration).SetEase(ease));
        }

        private bool IsSphereNear(long frame)
        {
            return SphereOffset() >= -_config.DoorOpenDistance;
        }

        private bool IsSphereThrough(long frame)
        {
            return SphereOffset() >= _config.DoorEnterDistance * .5f;
        }

        private float SphereOffset()
        {
            Vector3 delta = (_mainSphere.transform.position - transform.position).Flat();

            return Vector3.Dot(delta, _finish.Direction);
        }
    }
}

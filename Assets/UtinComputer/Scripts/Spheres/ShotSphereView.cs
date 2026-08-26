using UniRx;
using UnityEngine;
using UtinComputer.Spheres.Charges;
using Zenject;
namespace UtinComputer.Spheres
{
    public class ShotSphereView : MonoBehaviour
    {
        [SerializeField] private Transform body;
        [SerializeField] private Vector3 direction = Vector3.forward;

        private ChargeController _charge;

        [Inject]
        public void Construct(ChargeController charge)
        {
            _charge = charge;
        }

        private void Start()
        {
            _charge.IsCharging.Subscribe(OnCharging).AddTo(this);

            Observable.EveryUpdate()
                .Where(IsCharging)
                .Subscribe(OnChargeUpdate)
                .AddTo(this);
        }

        private void OnCharging(bool charging)
        {
            body.gameObject.SetActive(charging);
        }

        private void OnChargeUpdate(long frame)
        {
            float radius = _charge.ShotRadius.Value;

            body.localScale = Vector3.one * (radius * 2f);
            body.localPosition = direction.normalized * (_charge.Radius.Value + radius);
        }

        private bool IsCharging(long frame)
        {
            return _charge.IsCharging.Value;
        }
    }
}

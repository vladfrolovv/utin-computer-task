using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
namespace UtinComputer.Spheres.Charges
{
    public class SphereChargeInputView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private SphereChargeController _sphereCharge;

        [Inject]
        public void Construct(SphereChargeController sphereCharge)
        {
            _sphereCharge = sphereCharge;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _sphereCharge.BeginCharge();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _sphereCharge.EndCharge();
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
namespace UtinComputer.Spheres.Charges
{
    public class ChargeInputView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private ChargeController _charge;

        [Inject]
        public void Construct(ChargeController charge)
        {
            _charge = charge;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _charge.BeginCharge();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _charge.EndCharge();
        }
    }
}

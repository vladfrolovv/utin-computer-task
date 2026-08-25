using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
namespace UtinComputer.UI
{
    public class PlayButtonView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform foregroundRectTransform;
        [SerializeField] private RectTransform backgroundRectTransform;

        [Header("Animation Settings")]
        [SerializeField] private float moveDownTime = .16f;
        [SerializeField] private float moveUpTime = .16f;

        private float _foregroundYAmplitude;
        private Tween _foregroundTween;

        private void Awake()
        {
            _foregroundYAmplitude = backgroundRectTransform.anchoredPosition.y;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _foregroundTween?.Kill();
            _foregroundTween = foregroundRectTransform.DOLocalMoveY(_foregroundYAmplitude, moveDownTime).SetEase(Ease.OutQuint);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _foregroundTween?.Kill();
            _foregroundTween = foregroundRectTransform.DOLocalMoveY(0, moveDownTime).SetEase(Ease.OutQuint);
        }
    }
}

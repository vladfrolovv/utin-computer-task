using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
namespace UtinComputer.UI
{
    public class PlayButtonView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private RectTransform foregroundRectTransform;
        [SerializeField] private RectTransform backgroundRectTransform;
        [SerializeField] private TMP_Text label;

        [Header("Animation Settings")]
        [SerializeField] private float moveDownTime = .16f;
        [SerializeField] private float moveUpTime = .16f;

        private readonly Subject<Unit> _clicked = new();

        private float _foregroundYAmplitude;
        private Tween _foregroundTween;

        public IObservable<Unit> Clicked => _clicked;

        public void SetLabel(string text)
        {
            label.text = text;
        }

        private void Awake()
        {
            _foregroundYAmplitude = backgroundRectTransform.anchoredPosition.y;
        }

        private void OnDestroy()
        {
            _foregroundTween?.Kill();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _foregroundTween?.Kill();
            _foregroundTween = foregroundRectTransform.DOLocalMoveY(_foregroundYAmplitude, moveDownTime).SetEase(Ease.OutQuint);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _foregroundTween?.Kill();
            _foregroundTween = foregroundRectTransform.DOLocalMoveY(0f, moveUpTime).SetEase(Ease.OutQuint);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _clicked.OnNext(Unit.Default);
        }
    }
}

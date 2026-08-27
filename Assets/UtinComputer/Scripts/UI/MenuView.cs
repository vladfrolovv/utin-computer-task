using DG.Tweening;
using UniRx;
using UnityEngine;
using UtinComputer.Finish;
using Zenject;
namespace UtinComputer.UI
{
    public class MenuView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private PlayButtonView playButton;
        [SerializeField] private RectTransform winPopup;

        [Header("Labels")]
        [SerializeField] private string playLabel = "PLAY";
        [SerializeField] private string winLabel = "NEXT";
        [SerializeField] private string loseLabel = "TRY AGAIN";

        [Header("Animation Settings")]
        [SerializeField] private float popupTime = .45f;
        [SerializeField] private float fadeTime = .25f;

        private GameFlowController _gameFlow;
        private FinishController _finish;
        private Tween _fadeTween;
        private Tween _popupTween;

        [Inject]
        public void Construct(GameFlowController gameFlow, FinishController finish)
        {
            _gameFlow = gameFlow;
            _finish = finish;
        }

        private void Start()
        {
            winPopup.gameObject.SetActive(false);

            playButton.SetLabel(playLabel);

            playButton.Clicked.Subscribe(OnPlayClicked).AddTo(this);

            _gameFlow.IsPlaying.Subscribe(OnPlaying).AddTo(this);
            _finish.Finished.Subscribe(OnFinished).AddTo(this);
        }

        private void OnDestroy()
        {
            _fadeTween?.Kill();
            _popupTween?.Kill();
        }

        private void OnPlayClicked(Unit unit)
        {
            _gameFlow.Play();
        }

        private void OnPlaying(bool playing)
        {
            SetVisible(!playing);
        }

        private void OnFinished(FinishOutcome outcome)
        {
            bool won = outcome == FinishOutcome.Win;
            RectTransform popup = won ? winPopup : null;

            playButton.SetLabel(won ? winLabel : loseLabel);

            SetVisible(true);

            if (won)
            {
                popup.localScale = Vector3.zero;
                popup.gameObject.SetActive(true);

                _popupTween?.Kill();
                _popupTween = popup.DOScale(Vector3.one, popupTime).SetEase(Ease.OutBack);
            }
        }

        private void SetVisible(bool visible)
        {
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;

            _fadeTween?.Kill();
            _fadeTween = canvasGroup.DOFade(visible ? 1f : 0f, fadeTime);
        }
    }
}

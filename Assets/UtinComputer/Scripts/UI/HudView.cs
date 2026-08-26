using DG.Tweening;
using UniRx;
using UnityEngine;
using UtinComputer.Finish;
using Zenject;
namespace UtinComputer.UI
{
    public class HudView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Animation Settings")]
        [SerializeField] private float fadeTime = .25f;

        private GameFlowController _gameFlow;
        private Tween _fadeTween;

        [Inject]
        public void Construct(GameFlowController gameFlow)
        {
            _gameFlow = gameFlow;
        }

        private void Start()
        {
            _gameFlow.IsPlaying.Subscribe(OnPlaying).AddTo(this);
        }

        private void OnDestroy()
        {
            _fadeTween?.Kill();
        }

        private void OnPlaying(bool playing)
        {
            canvasGroup.interactable = playing;
            canvasGroup.blocksRaycasts = playing;

            _fadeTween?.Kill();
            _fadeTween = canvasGroup.DOFade(playing ? 1f : 0f, fadeTime);
        }
    }
}

using DG.Tweening;
using UnityEngine;
using UtinComputer.Core;
namespace UtinComputer.Effects
{
    public class BlastWaveView : MonoBehaviour
    {
        private static readonly int ProgressId = UnityEngine.Shader.PropertyToID("_Progress");
        private const float PlaneMeshSize = 10f;

        [SerializeField] private MeshRenderer meshRenderer;

        private MaterialPropertyBlock _propertyBlock;
        private Tween _waveTween;

        public void Play(BlastInfo blast, float groundHeight)
        {
            _propertyBlock ??= new MaterialPropertyBlock();

            transform.position = new Vector3(blast.Origin.x, groundHeight, blast.Origin.z);
            transform.localScale = Vector3.one * (blast.Radius * 2f / PlaneMeshSize);

            meshRenderer.enabled = true;
            SetProgress(0f);

            _waveTween?.Kill();
            _waveTween = DOVirtual.Float(0f, 1f, blast.Duration, SetProgress)
                .SetEase(Ease.Linear)
                .OnComplete(Hide);
        }

        private void Awake()
        {
            meshRenderer.enabled = false;
        }

        private void OnDestroy()
        {
            _waveTween?.Kill();
        }

        private void SetProgress(float progress)
        {
            _propertyBlock.SetFloat(ProgressId, progress);
            meshRenderer.SetPropertyBlock(_propertyBlock);
        }

        private void Hide()
        {
            meshRenderer.enabled = false;
        }
    }
}

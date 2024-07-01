using System.Collections;
using GameFlow.Component;
using UnityEngine;

namespace GameFlow
{
    [AddComponentMenu("Game Flow/UI Fade Animation")]
    public class FadeUIAnimation : GameFlowUIAnimation
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private bool ignoreTimeScale;
        [SerializeField] private float durationShow = 0.3f;
        [SerializeField] private float durationHide = 0.3f;

        protected override void OnShow()
        {
            if (durationShow == 0)
            {
                canvasGroup.alpha = 1;
                OnShowCompleted();
                return;
            }

            StartCoroutine(IEShow());
        }

        protected override void OnHide()
        {
            if (durationHide == 0)
            {
                canvasGroup.alpha = 0;
                OnHideCompleted();
                return;
            }

            StartCoroutine(IEHide());
        }

        private IEnumerator IEShow()
        {
            canvasGroup.alpha = 0;
            var time = 0f;
            do
            {
                yield return null;
                time += (ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) / durationShow;
                canvasGroup.alpha = time;
            } while (time < 1f);

            canvasGroup.alpha = 1;
            OnShowCompleted();
        }

        private IEnumerator IEHide()
        {
            canvasGroup.alpha = 1;
            var time = 0f;
            do
            {
                yield return null;
                time += (ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) / durationShow;
                canvasGroup.alpha = 1 - time;
            } while (time < 1f);

            canvasGroup.alpha = 0;
            OnHideCompleted();
        }
    }
}
using System.Collections;
using GameFlow.Component;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameFlow
{
    [AddComponentMenu("Game Flow/UI Fade Animation")]
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeUIAnimation : GameFlowUIAnimation
    {
        [SerializeField, FormerlySerializedAs("ignoreTimeScale")] private bool m_ignoreTimeScale;
        [SerializeField, FormerlySerializedAs("durationShow")] private float m_durationShow = 0.3f;
        [SerializeField, FormerlySerializedAs("durationHide")] private float m_durationHide = 0.3f;
        private CanvasGroup _canvasGroup;

        protected override void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            base.Awake();
        }

        protected override void OnShow()
        {
            if (m_durationShow == 0)
            {
                _canvasGroup.alpha = 1;
                OnShowCompleted();
                return;
            }

            StartCoroutine(IEShow());
        }

        protected override void OnHide()
        {
            if (m_durationHide == 0)
            {
                _canvasGroup.alpha = 0;
                OnHideCompleted();
                return;
            }

            StartCoroutine(IEHide());
        }

        private IEnumerator IEShow()
        {
            _canvasGroup.alpha = 0;
            var time = 0f;
            do
            {
                yield return null;
                time += (m_ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) / m_durationShow;
                _canvasGroup.alpha = time;
            } while (time < 1f);

            _canvasGroup.alpha = 1;
            OnShowCompleted();
        }

        private IEnumerator IEHide()
        {
            _canvasGroup.alpha = 1;
            var time = 0f;
            do
            {
                yield return null;
                time += (m_ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) / m_durationShow;
                _canvasGroup.alpha = 1 - time;
            } while (time < 1f);

            _canvasGroup.alpha = 0;
            OnHideCompleted();
        }
    }
}
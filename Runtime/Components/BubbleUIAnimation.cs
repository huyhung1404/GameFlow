using System.Collections;
using GameFlow.Component;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameFlow
{
    [AddComponentMenu("Game Flow/UI Bubble Animation")]
    public sealed class BubbleUIAnimation : GameFlowUIAnimation
    {
        [SerializeField, FormerlySerializedAs("ignoreTimeScale")] private bool m_ignoreTimeScale;
        [SerializeField, FormerlySerializedAs("view")] private Transform m_view;
        [SerializeField, FormerlySerializedAs("startScale")] private Vector3 m_startScale = new Vector3(0.9f, 0.9f, 1f);
        [SerializeField, FormerlySerializedAs("durationShow")] private float m_durationShow = 0.25f;
        [SerializeField, FormerlySerializedAs("durationHide")] private float m_durationHide = 0.15f;

        [SerializeField, FormerlySerializedAs("curve")] private AnimationCurve m_curve = new AnimationCurve(new[]
        {
            new Keyframe(0f, 0.002441406f, 8.881391f, 8.881391f),
            new Keyframe(0.1966633f, 1.066512f, 2.71777f, 2.71777f),
            new Keyframe(0.5494144f, 1.075302f, -1.172683f, -1.172683f),
            new Keyframe(0.879574f, 0.8086627f, 0.2559986f, 0.2559986f),
            new Keyframe(1.001953f, 1.004486f, 2.678603f, 2.678603f)
        });

        protected override void OnShow()
        {
            StartCoroutine(IEShow());
        }

        protected override void OnHide()
        {
            StartCoroutine(IEHide());
        }

        private IEnumerator IEShow()
        {
            m_view.localScale = m_startScale;
            var scale = Vector3.one - m_startScale;
            var time = 0f;
            do
            {
                yield return null;
                time += (m_ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) / m_durationShow;
                m_view.localScale = m_startScale + scale * m_curve.Evaluate(time);
            } while (time < 1f);

            m_view.localScale = Vector3.one;
            OnShowCompleted();
        }

        private IEnumerator IEHide()
        {
            var executeTime = (m_ignoreTimeScale ? Time.realtimeSinceStartup : Time.time) + m_durationHide;
            do
            {
                yield return null;
            } while ((m_ignoreTimeScale ? Time.realtimeSinceStartup : Time.time) < executeTime);

            OnHideCompleted();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopAllCoroutines();
            OnHideCompleted();
        }
    }
}
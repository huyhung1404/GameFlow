using System.Collections;
using GameFlow.Component;
using UnityEngine;

namespace GameFlow
{
    [AddComponentMenu("Game Flow/UI Simple Animation")]
    public class SimpleUIAnimation : GameFlowUIAnimation
    {
        [SerializeField] private bool ignoreTimeScale;

        [Header("Show")] [SerializeField] private Vector3 startScale = new Vector3(0.9f, 0.9f, 1f);
        [SerializeField] private float durationShow = 0.25f;

        [SerializeField] private AnimationCurve curve = new AnimationCurve(new[]
        {
            new Keyframe(0f, 0.002441406f, 8.881391f, 8.881391f),
            new Keyframe(0.1966633f, 1.066512f, 2.71777f, 2.71777f),
            new Keyframe(0.5494144f, 1.075302f, -1.172683f, -1.172683f),
            new Keyframe(0.879574f, 0.8086627f, 0.2559986f, 0.2559986f),
            new Keyframe(1.001953f, 1.004486f, 2.678603f, 2.678603f)
        });

        [Header("Hide")] [SerializeField] private float durationHide = 0.15f;

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
            yield return null;
        }

        protected IEnumerator IEHide()
        {
            var executeTime = (ignoreTimeScale ? Time.realtimeSinceStartup : Time.time) + durationHide;
            do
            {
                yield return null;
            } while ((ignoreTimeScale ? Time.realtimeSinceStartup : Time.time) < executeTime);

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
using GameFlow.Internal;
using UnityEngine;

namespace GameFlow
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeLoading : BaseLoadingTypeController
    {
        [SerializeField] private float defaultFadeTime = 0.3f;

        private CanvasGroup canvasGroup;
        private bool isShowing;
        private bool isHiding;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void OnShow()
        {
            timeExecute = defaultFadeTime;
            isShow = true;
            isShowing = true;
            isHiding = false;
        }

        protected override void OnHide()
        {
            timeExecute = defaultFadeTime;
            isHiding = true;
        }

        private void LateUpdate()
        {
            if (isShowing)
            {
                HandleShowing();
                return;
            }

            if (!isHiding) return;
            HandleHiding();
        }

        private void HandleShowing()
        {
            var alpha = canvasGroup.alpha;
            if (alpha >= 1)
            {
                isShowing = false;
                ExecuteCallback();
                return;
            }

            var speed = 1 / timeExecute;
            canvasGroup.alpha = Mathf.Clamp01(alpha + speed * Time.deltaTime);
        }

        private void HandleHiding()
        {
            var alpha = canvasGroup.alpha;
            if (alpha <= 0)
            {
                isHiding = false;
                isShow = false;
                ExecuteCallback();
                return;
            }

            var speed = 1 / timeExecute;
            canvasGroup.alpha = Mathf.Clamp01(alpha - speed * Time.deltaTime);
        }

        internal void LoadingIsShow()
        {
            Assert.IsTrue(gameObject.activeSelf);
            Assert.IsTrue(canvasGroup.alpha >= 1);
        }

        internal void LoadingIsHide()
        {
            Assert.IsTrue(!gameObject.activeSelf);
            Assert.IsTrue(canvasGroup.alpha <= 0);
        }
    }
}
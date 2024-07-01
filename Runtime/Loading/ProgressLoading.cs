using UnityEngine;
using UnityEngine.UI;

namespace GameFlow
{
    [AddComponentMenu("Game Flow/Loading/Progress")]
    [RequireComponent(typeof(CanvasGroup))]
    public class ProgressLoading : BaseLoadingTypeController
    {
        [SerializeField] internal Slider progressSlider;
        [SerializeField] private float hideSpeed = 0.15f;
        private CanvasGroup canvasGroup;
        private float currentProgress;
        private float targetProgress;
        private float lastTime;
        private bool isShowing;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }

        protected override void OnShow()
        {
            ExecuteCallback();
            progressSlider.value = 0;
            currentProgress = targetProgress = 0;
            canvasGroup.alpha = 1;
            lastTime = Time.realtimeSinceStartup;
            isShow = isShowing = true;
            timeExecute = hideSpeed;
        }

        protected override void OnHide()
        {
            isShowing = false;
            progressSlider.value = 1;
        }

        private void LateUpdate()
        {
            if (!isShowing)
            {
                var alpha = canvasGroup.alpha;
                if (alpha <= 0) return;
                var speed = 1 / timeExecute;
                alpha = Mathf.Clamp01(alpha - speed * Time.deltaTime);
                canvasGroup.alpha = alpha;
                if (alpha > 0) return;
                isShow = false;
                ExecuteCallback();
                return;
            }

            currentProgress = Mathf.Lerp(currentProgress, targetProgress, (Time.realtimeSinceStartup - lastTime) * 0.9f);
            progressSlider.value = currentProgress;
            lastTime = targetProgress < 1 ? Time.realtimeSinceStartup : -1;
            if (currentProgress < 1) return;
            OnHide();
        }

        public void UpdateProgress(float value)
        {
            targetProgress = value;
        }
    }
}
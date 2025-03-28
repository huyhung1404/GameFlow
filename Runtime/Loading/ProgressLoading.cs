using UnityEngine;

namespace GameFlow
{
    [AddComponentMenu("Game Flow/Loading/Progress")]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ProgressLoading : BaseLoadingTypeController
    {
        [SerializeField] private float hideSpeed = 0.15f;
        private CanvasGroup canvasGroup;
        private float currentProgress;
        private float targetProgress;
        private float lastTime;
        private bool isShowing;

        protected override void OnShow()
        {
            SetUpCanvasGroupIfNeed();
            ExecuteCallback();
            SetValue(0);
            currentProgress = targetProgress = 0;
            canvasGroup.alpha = 1;
            lastTime = Time.realtimeSinceStartup;
            isShow = isShowing = true;
            timeExecute = hideSpeed;
        }

        private void SetUpCanvasGroupIfNeed()
        {
            if (!ReferenceEquals(canvasGroup, null)) return;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void OnHide()
        {
            isShowing = false;
            SetValue(1);
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
            SetValue(currentProgress);
            lastTime = targetProgress < 1 ? Time.realtimeSinceStartup : -1;
            if (currentProgress < 1) return;
            OnHide();
        }

        public void UpdateProgress(float value)
        {
            targetProgress = value;
        }

        public void ForceHide()
        {
            targetProgress = currentProgress = 1;
            isShowing = false;
            isShow = false;
            ExecuteCallback();
            gameObject.SetActive(false);
        }

        protected abstract void SetValue(float value);
    }
}
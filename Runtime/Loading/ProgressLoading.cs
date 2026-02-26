using UnityEngine;
using UnityEngine.Serialization;

namespace GameFlow
{
    [AddComponentMenu("Game Flow/Loading/Progress")]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ProgressLoading : BaseLoadingTypeController
    {
        [SerializeField, FormerlySerializedAs("hideSpeed")] private float m_hideSpeed = 0.15f;
        [SerializeField, FormerlySerializedAs("moveSpeed")] private float m_moveSpeed = 0.15f;
        private CanvasGroup _canvasGroup;
        private float _currentProgress;
        private float _targetProgress;
        private bool _isShowing;

        protected override void OnShow()
        {
            SetUpCanvasGroupIfNeed();
            ExecuteCallback();
            SetValue(0);
            _currentProgress = _targetProgress = 0;
            _canvasGroup.alpha = 1;
            IsShow = _isShowing = true;
            _timeExecute = m_hideSpeed;
        }

        private void SetUpCanvasGroupIfNeed()
        {
            if (!ReferenceEquals(_canvasGroup, null)) return;
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void OnHide()
        {
            _isShowing = false;
            SetValue(1);
        }

        private void LateUpdate()
        {
            if (!_isShowing)
            {
                var alpha = _canvasGroup.alpha;
                if (alpha <= 0) return;
                var speed = 1 / _timeExecute;
                alpha = Mathf.Clamp01(alpha - speed * Time.deltaTime);
                _canvasGroup.alpha = alpha;
                if (alpha > 0) return;
                IsShow = false;
                ExecuteCallback();
                return;
            }

            _currentProgress = LerpProgress();
            SetValue(_currentProgress);
            if (_currentProgress < 1) return;
            OnHide();
        }

        public virtual float LerpProgress()
        {
            return Mathf.Clamp(_currentProgress + Time.deltaTime * m_moveSpeed, 0, _targetProgress);
        }

        public void UpdateProgress(float value)
        {
            _targetProgress = value;
        }
        
        public float CurrentProgress() => _currentProgress;
        public float TargetProgress() => _targetProgress;

        public void ForceHide()
        {
            _targetProgress = _currentProgress = 1;
            _isShowing = false;
            IsShow = false;
            ExecuteCallback();
            gameObject.SetActive(false);
        }

        protected abstract void SetValue(float value);
    }
}
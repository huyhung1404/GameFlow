using System;
using GameFlow.Internal;
using UnityEngine;

namespace GameFlow
{
    [AddComponentMenu("Game Flow/Loading/Fade")]
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeLoading : BaseLoadingTypeController
    {
        [SerializeField] private float m_defaultFadeTime = 0.3f;
        private CanvasGroup _canvasGroup;
        private bool _isShowing;
        private bool _isHiding;
        private float _timeExecuteHide;
        private bool _isCacheShowCallback;

        protected override void OnShow()
        {
            SetUpCanvasGroupIfNeed();
            _timeExecute = m_defaultFadeTime;
            IsShow = true;
            _isShowing = true;
            _isHiding = false;
        }

        private void SetUpCanvasGroupIfNeed()
        {
            if (!ReferenceEquals(_canvasGroup, null)) return;
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
        }

        protected override void OnHide()
        {
            if (!IsEnable && !IsShow)
            {
                _isHiding = false;
                IsShow = false;
                return;
            }

            _timeExecuteHide = m_defaultFadeTime;
            _isHiding = true;
        }

        private void LateUpdate()
        {
            if (_isShowing)
            {
                HandleShowing();
                return;
            }

            if (!_isHiding) return;
            HandleHiding();
        }

        private void HandleShowing()
        {
            var alpha = _canvasGroup.alpha;
            if (alpha >= 1)
            {
                _isShowing = false;
                ExecuteCallback();
                return;
            }

            var speed = 1 / _timeExecute;
            _canvasGroup.alpha = Mathf.Clamp01(alpha + speed * Time.deltaTime);
        }

        private void HandleHiding()
        {
            var alpha = _canvasGroup.alpha;
            if (alpha <= 0)
            {
                _isHiding = false;
                IsShow = false;
                ExecuteCallback();
                return;
            }

            var speed = 1 / _timeExecuteHide;
            _canvasGroup.alpha = Mathf.Clamp01(alpha - speed * Time.deltaTime);
        }

        /// <summary>
        /// Assigns the onCompleted Action to the loading sequence.  
        /// If a previous event exists, it will be executed before overwriting with the new condition.
        /// </summary>
        /// <param name="onCompleted">The Action to be invoked when the animation is completed</param>
        /// <returns></returns>
        public override BaseLoadingTypeController OnCompleted(Action onCompleted)
        {
            if (!IsEnable && !_isShowing && !IsShow)
            {
                _callback = onCompleted;
                ExecuteCallback();
                return this;
            }

            if (!_isShowing || !_isHiding || _callback == null)
            {
                if (ExecuteCallback()) ExecuteCallback();
                _callback = onCompleted;
                return this;
            }

            if (_isCacheShowCallback)
            {
                _callback += onCompleted;
                return this;
            }

            _isCacheShowCallback = true;
            var showingCallback = _callback;
            _callback = () =>
            {
                try
                {
                    showingCallback.Invoke();
                }
                catch (Exception e)
                {
                    ErrorHandle.LogException(e, "Loading Callback");
                }

                _callback = onCompleted;
                _cacheCallback = true;
                _isCacheShowCallback = false;
            };
            return this;
        }

        public override BaseLoadingTypeController SetTime(float time)
        {
            if (_isShowing && !_isHiding) _timeExecute = time;
            if (_isHiding) _timeExecuteHide = time;
            return this;
        }

        internal void LoadingIsShow()
        {
            Assert.IsTrue(gameObject.activeSelf, "gameObject.activeSelf");
            Assert.IsTrue(_canvasGroup.alpha >= 1, "canvasGroup.alpha >= 1");
        }

        internal void LoadingIsHide()
        {
            Assert.IsTrue(!gameObject.activeSelf, "!gameObject.activeSelf");
            Assert.IsTrue(_canvasGroup.alpha <= 0, "canvasGroup.alpha <= 0");
        }
    }
}
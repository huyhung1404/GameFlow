﻿using System;
using GameFlow.Internal;
using UnityEngine;

namespace GameFlow
{
    [AddComponentMenu("Game Flow/Loading/Fade")]
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeLoading : BaseLoadingTypeController
    {
        [SerializeField] private float defaultFadeTime = 0.3f;
        private CanvasGroup canvasGroup;
        private bool isShowing;
        private bool isHiding;
        private float timeExecuteHide;
        private bool isCacheShowCallback;

        protected override void OnShow()
        {
            SetUpCanvasGroupIfNeed();
            timeExecute = defaultFadeTime;
            isShow = true;
            isShowing = true;
            isHiding = false;
        }

        private void SetUpCanvasGroupIfNeed()
        {
            if (!ReferenceEquals(canvasGroup, null)) return;
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }

        protected override void OnHide()
        {
            if (!isEnable && !isShow)
            {
                isHiding = false;
                isShow = false;
                return;
            }

            timeExecuteHide = defaultFadeTime;
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

            var speed = 1 / timeExecuteHide;
            canvasGroup.alpha = Mathf.Clamp01(alpha - speed * Time.deltaTime);
        }

        /// <summary>
        /// Assigns the onCompleted Action to the loading sequence.  
        /// If a previous event exists, it will be executed before overwriting with the new condition.
        /// </summary>
        /// <param name="onCompleted">The Action to be invoked when the animation is completed</param>
        /// <returns></returns>
        public override BaseLoadingTypeController OnCompleted(Action onCompleted)
        {
            if (!isEnable && !isShowing && !isShow)
            {
                callback = onCompleted;
                ExecuteCallback();
                return this;
            }

            if (!isShowing || !isHiding || callback == null)
            {
                if (ExecuteCallback()) ExecuteCallback();
                callback = onCompleted;
                return this;
            }

            if (isCacheShowCallback)
            {
                callback += onCompleted;
                return this;
            }

            isCacheShowCallback = true;
            var showingCallback = callback;
            callback = () =>
            {
                try
                {
                    showingCallback.Invoke();
                }
                catch (Exception e)
                {
                    ErrorHandle.LogException(e, "Loading Callback");
                }

                callback = onCompleted;
                cacheCallback = true;
                isCacheShowCallback = false;
            };
            return this;
        }

        public override BaseLoadingTypeController SetTime(float time)
        {
            if (isShowing && !isHiding) timeExecute = time;
            if (isHiding) timeExecuteHide = time;
            return this;
        }

        internal void LoadingIsShow()
        {
            Assert.IsTrue(gameObject.activeSelf, "gameObject.activeSelf");
            Assert.IsTrue(canvasGroup.alpha >= 1, "canvasGroup.alpha >= 1");
        }

        internal void LoadingIsHide()
        {
            Assert.IsTrue(!gameObject.activeSelf, "!gameObject.activeSelf");
            Assert.IsTrue(canvasGroup.alpha <= 0, "canvasGroup.alpha <= 0");
        }
    }
}
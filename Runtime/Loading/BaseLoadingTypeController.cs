using System;
using GameFlow.Internal;
using UnityEngine;

namespace GameFlow
{
    public abstract class BaseLoadingTypeController : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool isShow { get; protected set; }
#else
        public bool isShow;
#endif

#if UNITY_EDITOR
        public bool isEnable { get; private set; }
#else
        public bool isEnable;
#endif
        protected Action callback;
        protected float timeExecute;

        private void OnEnable()
        {
            isEnable = true;
        }

        private void OnDisable()
        {
            isEnable = false;
        }

        internal BaseLoadingTypeController On()
        {
            OnShow();
            return this;
        }

        internal BaseLoadingTypeController Off()
        {
            OnHide();
            return this;
        }

        protected abstract void OnShow();
        protected abstract void OnHide();

        public BaseLoadingTypeController OnCompleted(Action onCompleted)
        {
            if (callback != null)
            {
                try
                {
                    callback.Invoke();
                }
                catch (Exception e)
                {
                    ErrorHandle.LogException(e, "Loading Callback");
                }
            }

            callback = onCompleted;
            return this;
        }

        public BaseLoadingTypeController SetTime(float time)
        {
            timeExecute = time;
            return this;
        }

        protected void ExecuteCallback()
        {
            if (callback == null) return;
            try
            {
                callback.Invoke();
                callback = null;
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Execute Callback");
            }
        }
    }
}
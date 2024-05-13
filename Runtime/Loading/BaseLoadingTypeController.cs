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
        protected bool cacheCallback;
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

        public virtual BaseLoadingTypeController OnCompleted(Action onCompleted)
        {
            ExecuteCallback();
            callback = onCompleted;
            return this;
        }

        public virtual BaseLoadingTypeController SetTime(float time)
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
                if (cacheCallback)
                {
                    cacheCallback = false;
                    return;
                }

                callback = null;
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Execute Callback");
            }
        }
    }
}
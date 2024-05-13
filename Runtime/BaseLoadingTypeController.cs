using System;
using GameFlow.Internal;
using UnityEngine;

namespace GameFlow
{
    public abstract class BaseLoadingTypeController : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool isShow { get; private set; }
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
            Enable();
        }

        protected abstract void Enable();

        private void OnDisable()
        {
            isEnable = false;
            Disable();
        }

        protected abstract void Disable();

        internal BaseLoadingTypeController On()
        {
            isShow = true;
            return this;
        }

        internal BaseLoadingTypeController Off()
        {
            isShow = false;
            return this;
        }

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
    }
}
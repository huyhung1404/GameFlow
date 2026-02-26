using System;
using GameFlow.Internal;
using UnityEngine;

namespace GameFlow
{
    public abstract class BaseLoadingTypeController : MonoBehaviour
    {
        public bool IsShow { get; protected set; }
        public bool IsEnable { get; private set; }
        protected Action _callback;
        protected bool _cacheCallback;
        protected float _timeExecute;

        protected virtual void OnEnable()
        {
            IsEnable = true;
        }

        protected virtual void OnDisable()
        {
            IsEnable = false;
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
            _callback = onCompleted;
            return this;
        }

        public virtual BaseLoadingTypeController SetTime(float time)
        {
            _timeExecute = time;
            return this;
        }

        protected bool ExecuteCallback()
        {
            if (_callback == null) return false;
            try
            {
                _callback.Invoke();
                if (_cacheCallback)
                {
                    _cacheCallback = false;
                    return true;
                }

                _callback = null;
                return true;
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Execute Callback");
            }

            return false;
        }
    }
}
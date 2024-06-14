using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class ElementCallbackEvent
    {
        private Action onActive;
        public event Action OnActive { add => onActive += value; remove => onActive -= value; }

        internal void RaiseOnActive()
        {
            try
            {
                onActive?.Invoke();
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Callback OnActive Error");
            }
        }

        private Action<object> onActiveWithData;
        public event Action<object> OnActiveWithData { add => onActiveWithData += value; remove => onActiveWithData -= value; }

        internal void RaiseOnActiveWithData(object data)
        {
            try
            {
                onActiveWithData?.Invoke(data);
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Callback OnActive With Data Error");
            }
        }

        private Action<bool> onRelease;

        /// <summary>
        /// Return true if release immediately
        /// </summary>
        public event Action<bool> OnRelease { add => onRelease += value; remove => onRelease -= value; }

        internal void RaiseOnRelease(bool isReleaseImmediately)
        {
            try
            {
                onRelease?.Invoke(isReleaseImmediately);
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Callback OnRelease Error");
            }
        }
    }

    public class UIElementCallbackEvent : ElementCallbackEvent
    {
        private Action onShowCompleted;
        public event Action OnShowCompleted { add => onShowCompleted += value; remove => onShowCompleted -= value; }

        internal void RaiseOnShowCompleted()
        {
            try
            {
                onShowCompleted?.Invoke();
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Callback OnShowCompleted Error");
            }
        }

        private Action<ICommandReleaseHandle> onHide;
        public event Action<ICommandReleaseHandle> OnHide { add => onHide += value; remove => onHide -= value; }

        internal bool RaiseOnHide(ICommandReleaseHandle handle)
        {
            if (onHide == null) return false;
            try
            {
                onHide.Invoke(handle);
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Callback OnShowCompleted Error");
            }

            return true;
        }
    }
}
using System;
using System.Linq;
using GameFlow.Internal;

namespace GameFlow
{
    public class ElementCallbackEvent
    {
        protected Action onActive;
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

        protected Action<object> onActiveWithData;
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

        protected Action<bool> onRelease;

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

        internal virtual void GetInfo(out bool isUserInterface, out int callbackCount)
        {
            isUserInterface = false;
            callbackCount = 0;
            if (onActive != null) callbackCount++;
            if (onActiveWithData != null) callbackCount++;
            if (onRelease != null) callbackCount++;
        }

        public override string ToString()
        {
            return $"<b><size=11>OnActive</size></b>                    {(onActive == null ? "Event: 0" : GetDelegatesInfo(onActive.GetInvocationList()))}\n" +
                   $"<b><size=11>OnActiveWithData</size></b>    {(onActiveWithData == null ? "Event: 0" : GetDelegatesInfo(onActiveWithData.GetInvocationList()))}\n" +
                   $"<b><size=11>OnRelease</size></b>                 {(onRelease == null ? "Event: 0" : GetDelegatesInfo(onRelease.GetInvocationList()))}";
        }

        protected static string GetDelegatesInfo(Delegate[] delegates)
        {
            var result = $"Event: {delegates.Length}";
            return delegates.Aggregate(result, (current, variableDelegate)
                => current + $"\n{variableDelegate.Target}.{variableDelegate.Method.Name}");
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

        private Action onKeyBack;
        public event Action OnKeyBack { add => onKeyBack += value; remove => onKeyBack -= value; }

        internal void RaiseOnKeyBack()
        {
            try
            {
                onKeyBack?.Invoke();
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Callback OnKeyBack Error");
            }
        }

        private Action onReFocus;
        public event Action OnReFocus { add => onReFocus += value; remove => onReFocus -= value; }

        internal void RaiseOnReFocus()
        {
            try
            {
                onReFocus?.Invoke();
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Callback OnReFocus Error");
            }
        }

        internal override void GetInfo(out bool isUserInterface, out int callbackCount)
        {
            base.GetInfo(out isUserInterface, out callbackCount);
            isUserInterface = true;
            if (onShowCompleted != null) callbackCount++;
            if (onHide != null) callbackCount++;
            if (onKeyBack != null) callbackCount++;
            if (onReFocus != null) callbackCount++;
        }

        public override string ToString()
        {
            return $"<b><size=11>OnActive</size></b>                       {(onActive == null ? "Event: 0" : GetDelegatesInfo(onActive.GetInvocationList()))}\n" +
                   $"<b><size=11>OnActiveWithData</size></b>       {(onActiveWithData == null ? "Event: 0" : GetDelegatesInfo(onActiveWithData.GetInvocationList()))}\n" +
                   $"<b><size=11>OnShowCompleted</size></b>     {(onShowCompleted == null ? "Event: 0" : GetDelegatesInfo(onShowCompleted.GetInvocationList()))}\n" +
                   $"<b><size=11>OnHide</size></b>                          {(onHide == null ? "Event: 0" : GetDelegatesInfo(onHide.GetInvocationList()))}\n" +
                   $"<b><size=11>OnKeyBack</size></b>                    {(onKeyBack == null ? "Event: 0" : GetDelegatesInfo(onKeyBack.GetInvocationList()))}\n" +
                   $"<b><size=11>OnReFocus</size></b>                   {(onReFocus == null ? "Event: 0" : GetDelegatesInfo(onReFocus.GetInvocationList()))}\n" +
                   $"<b><size=11>OnRelease</size></b>                    {(onRelease == null ? "Event: 0" : GetDelegatesInfo(onRelease.GetInvocationList()))}";
        }
    }
}
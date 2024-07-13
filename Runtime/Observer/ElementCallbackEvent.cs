using System;
using System.Collections.Generic;
using System.Linq;
using GameFlow.Internal;

namespace GameFlow
{
    public class ElementCallbackEvent
    {
        protected List<FlowListenerMonoBehaviour> listeners;
        protected OnActive onActive;
        protected OnActiveWithData onActiveWithData;
        protected OnRelease onRelease;
        public event OnActive OnActive { add => onActive += value; remove => onActive -= value; }
        public event OnActiveWithData OnActiveWithData { add => onActiveWithData += value; remove => onActiveWithData -= value; }
        public event OnRelease OnRelease { add => onRelease += value; remove => onRelease -= value; }

        public void RegisterListener(FlowListenerMonoBehaviour listener)
        {
            listeners ??= new List<FlowListenerMonoBehaviour>();
            listeners.Add(listener);
        }

        public void UnregisterListener(FlowListenerMonoBehaviour listener)
        {
            listeners?.Remove(listener);
        }

        internal void RaiseOnActive()
        {
            try
            {
                onActive?.Invoke();
                if (listeners == null) return;
                var count = listeners.Count;
                for (var i = 0; i < count; i++)
                {
                    listeners[i].OnActive();
                }
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Callback OnActive Error");
            }
        }

        internal void RaiseOnActiveWithData(object data)
        {
            try
            {
                onActiveWithData?.Invoke(data);
                if (listeners == null) return;
                var count = listeners.Count;
                for (var i = 0; i < count; i++)
                {
                    listeners[i].OnActiveWithData(data);
                }
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Callback OnActive With Data Error");
            }
        }

        internal void RaiseOnRelease(bool isReleaseImmediately)
        {
            try
            {
                onRelease?.Invoke(isReleaseImmediately);
                if (listeners == null) return;
                var count = listeners.Count;
                for (var i = 0; i < count; i++)
                {
                    listeners[i].OnRelease(isReleaseImmediately);
                }
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Callback OnRelease Error");
            }
        }

        internal virtual void GetInfo(out bool isUserInterface, out int callbackCount, out int listenerCount)
        {
            isUserInterface = false;
            callbackCount = 0;
            listenerCount = listeners?.Count ?? 0;
            if (onActive != null) callbackCount++;
            if (onActiveWithData != null) callbackCount++;
            if (onRelease != null) callbackCount++;
        }

        public override string ToString()
        {
            return GetListenerInfo() + $"<b><size=11>OnActive</size></b>                    {(onActive == null ? "Event: 0" : GetDelegatesInfo(onActive.GetInvocationList()))}\n" +
                   $"<b><size=11>OnActiveWithData</size></b>    {(onActiveWithData == null ? "Event: 0" : GetDelegatesInfo(onActiveWithData.GetInvocationList()))}\n" +
                   $"<b><size=11>OnRelease</size></b>                 {(onRelease == null ? "Event: 0" : GetDelegatesInfo(onRelease.GetInvocationList()))}";
        }

        protected string GetListenerInfo()
        {
            var info = "<b><size=11>Listener:</size></b>\n";
            if (listeners == null) return info;
            for (var i = 0; i < listeners.Count; i++)
            {
                var listener = listeners[i];
                info += $"[{i}] {listener.name}\n";
            }

            return info;
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
        private OnShowCompleted onShowCompleted;
        private OnHide onHide;
        private OnKeyBack onKeyBack;
        private OnReFocus onReFocus;
        public event OnShowCompleted OnShowCompleted { add => onShowCompleted += value; remove => onShowCompleted -= value; }
        public event OnHide OnHide { add => onHide += value; remove => onHide -= value; }
        public event OnKeyBack OnKeyBack { add => onKeyBack += value; remove => onKeyBack -= value; }
        public event OnReFocus OnReFocus { add => onReFocus += value; remove => onReFocus -= value; }

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

        internal bool RaiseOnHide(ICommandReleaseHandle handle)
        {
            if (onHide == null) return false;
            try
            {
                onHide.Invoke(handle);
                return true;
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Callback OnShowCompleted Error");
            }

            return false;
        }

        internal void RaiseOnKeyBack()
        {
            try
            {
                onKeyBack?.Invoke();
                if (listeners == null) return;
                var count = listeners.Count;
                for (var i = 0; i < count; i++)
                {
                    listeners[i].OnKeyBack();
                }
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Callback OnKeyBack Error");
            }
        }


        internal void RaiseOnReFocus()
        {
            try
            {
                onReFocus?.Invoke();
                if (listeners == null) return;
                var count = listeners.Count;
                for (var i = 0; i < count; i++)
                {
                    listeners[i].OnReFocus();
                }
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, "Callback OnReFocus Error");
            }
        }

        internal override void GetInfo(out bool isUserInterface, out int callbackCount, out int listenerCount)
        {
            base.GetInfo(out isUserInterface, out callbackCount, out listenerCount);
            isUserInterface = true;
            if (onShowCompleted != null) callbackCount++;
            if (onHide != null) callbackCount++;
            if (onKeyBack != null) callbackCount++;
            if (onReFocus != null) callbackCount++;
        }

        public override string ToString()
        {
            return GetListenerInfo() + $"<b><size=11>OnActive</size></b>                       {(onActive == null ? "Event: 0" : GetDelegatesInfo(onActive.GetInvocationList()))}\n" +
                   $"<b><size=11>OnActiveWithData</size></b>       {(onActiveWithData == null ? "Event: 0" : GetDelegatesInfo(onActiveWithData.GetInvocationList()))}\n" +
                   $"<b><size=11>OnShowCompleted</size></b>     {(onShowCompleted == null ? "Event: 0" : GetDelegatesInfo(onShowCompleted.GetInvocationList()))}\n" +
                   $"<b><size=11>OnHide</size></b>                          {(onHide == null ? "Event: 0" : GetDelegatesInfo(onHide.GetInvocationList()))}\n" +
                   $"<b><size=11>OnKeyBack</size></b>                    {(onKeyBack == null ? "Event: 0" : GetDelegatesInfo(onKeyBack.GetInvocationList()))}\n" +
                   $"<b><size=11>OnReFocus</size></b>                   {(onReFocus == null ? "Event: 0" : GetDelegatesInfo(onReFocus.GetInvocationList()))}\n" +
                   $"<b><size=11>OnRelease</size></b>                    {(onRelease == null ? "Event: 0" : GetDelegatesInfo(onRelease.GetInvocationList()))}";
        }
    }
}
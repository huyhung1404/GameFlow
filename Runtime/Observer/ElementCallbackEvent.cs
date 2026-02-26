using System;
using System.Collections.Generic;
using System.Linq;
using GameFlow.Internal;

namespace GameFlow
{
    public class ElementCallbackEvent
    {
        protected List<IFlowListener> _listeners;
        protected OnActive _onActive;
        protected OnActiveWithData _onActiveWithData;
        protected OnRelease _onRelease;
        public event OnActive OnActive { add => _onActive += value; remove => _onActive -= value; }
        public event OnActiveWithData OnActiveWithData { add => _onActiveWithData += value; remove => _onActiveWithData -= value; }
        public event OnRelease OnRelease { add => _onRelease += value; remove => _onRelease -= value; }

        public void RegisterListener(IFlowListener listener)
        {
            _listeners ??= new List<IFlowListener>();
            _listeners.Add(listener);
        }

        public void UnregisterListener(IFlowListener listener)
        {
            _listeners?.Remove(listener);
        }

        internal void RaiseOnActive()
        {
            try
            {
                _onActive?.Invoke();
                if (_listeners == null) return;
                var count = _listeners.Count;
                for (var i = 0; i < count; i++)
                {
                    _listeners[i].OnActive();
                }
            }
            catch (Exception e)
            {
                LogError(e, "OnActive", _onActive?.GetInvocationList());
            }
        }

        internal void RaiseOnActiveWithData(object data)
        {
            try
            {
                _onActiveWithData?.Invoke(data);
                if (_listeners == null) return;
                var count = _listeners.Count;
                for (var i = 0; i < count; i++)
                {
                    _listeners[i].OnActiveWithData(data);
                }
            }
            catch (Exception e)
            {
                LogError(e, "OnActiveWithData", _onActiveWithData?.GetInvocationList());
            }
        }

        internal void RaiseOnRelease(bool isReleaseImmediately)
        {
            try
            {
                _onRelease?.Invoke(isReleaseImmediately);
                if (_listeners == null) return;
                var count = _listeners.Count;
                for (var i = 0; i < count; i++)
                {
                    _listeners[i].OnRelease(isReleaseImmediately);
                }
            }
            catch (Exception e)
            {
                LogError(e, "OnRelease", _onRelease?.GetInvocationList());
            }
        }

        internal virtual void GetInfo(out bool isUserInterface, out int callbackCount, out int listenerCount)
        {
            isUserInterface = false;
            callbackCount = 0;
            listenerCount = _listeners?.Count ?? 0;
            if (_onActive != null) callbackCount++;
            if (_onActiveWithData != null) callbackCount++;
            if (_onRelease != null) callbackCount++;
        }

        public override string ToString()
        {
            return GetListenerInfo() +
                   $"<b><size=11>OnActive</size></b>                    {(_onActive == null ? "Event: 0" : GetDelegatesInfo(_onActive.GetInvocationList()))}\n" +
                   $"<b><size=11>OnActiveWithData</size></b>    {(_onActiveWithData == null ? "Event: 0" : GetDelegatesInfo(_onActiveWithData.GetInvocationList()))}\n" +
                   $"<b><size=11>OnRelease</size></b>                 {(_onRelease == null ? "Event: 0" : GetDelegatesInfo(_onRelease.GetInvocationList()))}";
        }

        protected string GetListenerInfo()
        {
            var info = "<b><size=11>Listener:</size></b>\n";
            if (_listeners == null) return info;
            for (var i = 0; i < _listeners.Count; i++)
            {
                var listener = _listeners[i];
                info += $"[{i}] {listener}\n";
            }

            return info;
        }

        protected static string GetDelegatesInfo(Delegate[] delegates)
        {
            var result = $"Event: {delegates.Length}";
            return delegates.Aggregate(result, (current, variableDelegate)
                => current + $"\n{variableDelegate.Target}.{variableDelegate.Method.Name}");
        }

        protected void LogError(Exception e, string methodName, Delegate[] delegates)
        {
            var errorMessage = $"{methodName}:\n";
            if (delegates != null)
            {
                foreach (var method in delegates)
                {
                    errorMessage += $"Delegates: {method.Method}/{method.Target}\n";
                }
            }

            if (_listeners != null)
            {
                foreach (var listener in _listeners)
                {
                    errorMessage += $"Listener: {listener}";
                }
            }

            ErrorHandle.LogException(e, errorMessage);
        }
    }

    public class UIElementCallbackEvent : ElementCallbackEvent
    {
        private OnShowCompleted _onShowCompleted;
        private OnHide _onHide;
        private OnKeyBack _onKeyBack;
        private OnReFocus _onReFocus;
        public event OnShowCompleted OnShowCompleted { add => _onShowCompleted += value; remove => _onShowCompleted -= value; }
        public event OnHide OnHide { add => _onHide += value; remove => _onHide -= value; }
        public event OnKeyBack OnKeyBack { add => _onKeyBack += value; remove => _onKeyBack -= value; }
        public event OnReFocus OnReFocus { add => _onReFocus += value; remove => _onReFocus -= value; }

        internal void RaiseOnShowCompleted()
        {
            try
            {
                _onShowCompleted?.Invoke();
            }
            catch (Exception e)
            {
                LogError(e, "OnShowCompleted", _onShowCompleted?.GetInvocationList());
            }
        }

        internal bool RaiseOnHide(ICommandReleaseHandle handle)
        {
            if (_onHide == null) return false;
            try
            {
                _onHide.Invoke(handle);
                return true;
            }
            catch (Exception e)
            {
                LogError(e, "OnHide", _onHide?.GetInvocationList());
            }

            return false;
        }

        internal void RaiseOnKeyBack()
        {
            try
            {
                _onKeyBack?.Invoke();
                if (_listeners == null) return;
                var count = _listeners.Count;
                for (var i = 0; i < count; i++)
                {
                    _listeners[i].OnKeyBack();
                }
            }
            catch (Exception e)
            {
                LogError(e, "OnKeyBack", _onKeyBack?.GetInvocationList());
            }
        }


        internal void RaiseOnReFocus()
        {
            try
            {
                _onReFocus?.Invoke();
                if (_listeners == null) return;
                var count = _listeners.Count;
                for (var i = 0; i < count; i++)
                {
                    _listeners[i].OnReFocus();
                }
            }
            catch (Exception e)
            {
                LogError(e, "OnReFocus", _onReFocus?.GetInvocationList());
            }
        }

        internal override void GetInfo(out bool isUserInterface, out int callbackCount, out int listenerCount)
        {
            base.GetInfo(out isUserInterface, out callbackCount, out listenerCount);
            isUserInterface = true;
            if (_onShowCompleted != null) callbackCount++;
            if (_onHide != null) callbackCount++;
            if (_onKeyBack != null) callbackCount++;
            if (_onReFocus != null) callbackCount++;
        }

        public override string ToString()
        {
            return GetListenerInfo() +
                   $"<b><size=11>OnActive</size></b>                       {(_onActive == null ? "Event: 0" : GetDelegatesInfo(_onActive.GetInvocationList()))}\n" +
                   $"<b><size=11>OnActiveWithData</size></b>       {(_onActiveWithData == null ? "Event: 0" : GetDelegatesInfo(_onActiveWithData.GetInvocationList()))}\n" +
                   $"<b><size=11>OnShowCompleted</size></b>     {(_onShowCompleted == null ? "Event: 0" : GetDelegatesInfo(_onShowCompleted.GetInvocationList()))}\n" +
                   $"<b><size=11>OnHide</size></b>                          {(_onHide == null ? "Event: 0" : GetDelegatesInfo(_onHide.GetInvocationList()))}\n" +
                   $"<b><size=11>OnKeyBack</size></b>                    {(_onKeyBack == null ? "Event: 0" : GetDelegatesInfo(_onKeyBack.GetInvocationList()))}\n" +
                   $"<b><size=11>OnReFocus</size></b>                   {(_onReFocus == null ? "Event: 0" : GetDelegatesInfo(_onReFocus.GetInvocationList()))}\n" +
                   $"<b><size=11>OnRelease</size></b>                    {(_onRelease == null ? "Event: 0" : GetDelegatesInfo(_onRelease.GetInvocationList()))}";
        }
    }
}
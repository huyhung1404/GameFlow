using System;
using System.Collections.Generic;
using System.Linq;
using GameFlow.Internal;

namespace GameFlow
{
    public class ElementCallbackEvent
    {
        protected List<OnActive> onActive;

        public event OnActive OnActive
        {
            add
            {
                onActive ??= new List<OnActive>(3);
                onActive.Add(value);
            }
            remove
            {
                if (onActive == null) return;
                for (var i = onActive.Count - 1; i >= 0; i--)
                {
                    if (onActive[i] == value) onActive.RemoveAt(i);
                }
            }
        }

        internal void RaiseOnActive()
        {
            if (onActive == null) return;
            var count = onActive.Count;
            for (var i = 0; i < count; i++)
            {
                try
                {
                    onActive[i]?.Invoke();
                }
                catch (Exception e)
                {
                    ErrorHandle.LogException(e, "Callback OnActive Error");
                }
            }
        }

        protected List<OnActiveWithData> onActiveWithData;

        public event OnActiveWithData OnActiveWithData
        {
            add
            {
                onActiveWithData ??= new List<OnActiveWithData>(1);
                onActiveWithData.Add(value);
            }
            remove
            {
                if (onActiveWithData == null) return;
                for (var i = onActiveWithData.Count - 1; i >= 0; i--)
                {
                    if (onActiveWithData[i] == value) onActiveWithData.RemoveAt(i);
                }
            }
        }

        internal void RaiseOnActiveWithData(object data)
        {
            if (onActiveWithData == null) return;
            var count = onActiveWithData.Count;
            for (var i = 0; i < count; i++)
            {
                try
                {
                    onActiveWithData[i]?.Invoke(data);
                }
                catch (Exception e)
                {
                    ErrorHandle.LogException(e, "Callback OnActive With Data Error");
                }
            }
        }

        protected List<OnRelease> onRelease;

        public event OnRelease OnRelease
        {
            add
            {
                onRelease ??= new List<OnRelease>(1);
                onRelease.Add(value);
            }
            remove
            {
                if (onRelease == null) return;
                for (var i = onRelease.Count - 1; i >= 0; i--)
                {
                    if (onRelease[i] == value) onRelease.RemoveAt(i);
                }
            }
        }

        internal void RaiseOnRelease(bool isReleaseImmediately)
        {
            if (onRelease == null) return;
            var count = onRelease.Count;
            for (var i = 0; i < count; i++)
            {
                try
                {
                    onRelease[i]?.Invoke(isReleaseImmediately);
                }
                catch (Exception e)
                {
                    ErrorHandle.LogException(e, "Callback OnRelease Error");
                }
            }
        }

        public virtual void ClearDelegates(object target)
        {
            ClearDelegate(onActive, target);
            ClearDelegate(onActiveWithData, target);
            ClearDelegate(onRelease, target);
        }

        protected static void ClearDelegate<T>(List<T> delegates, object target) where T : Delegate
        {
            if (delegates == null) return;
            for (var i = delegates.Count - 1; i >= 0; i--)
            {
                if (delegates[i].Target == target) delegates.RemoveAt(i);
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
            return $"<b><size=11>OnActive</size></b>                    {(onActive == null ? "Event: 0" : GetDelegatesInfo(onActive))}\n" +
                   $"<b><size=11>OnActiveWithData</size></b>    {(onActiveWithData == null ? "Event: 0" : GetDelegatesInfo(onActiveWithData))}\n" +
                   $"<b><size=11>OnRelease</size></b>                 {(onRelease == null ? "Event: 0" : GetDelegatesInfo(onRelease))}";
        }

        protected static string GetDelegatesInfo<T>(List<T> delegates) where T : Delegate
        {
            var result = $"Event: {delegates.Count}";
            return delegates.Aggregate(result, (current, variableDelegate)
                => current + $"\n{variableDelegate.Target}.{variableDelegate.Method.Name}");
        }
    }

    public class UIElementCallbackEvent : ElementCallbackEvent
    {
        private List<OnShowCompleted> onShowCompleted;

        public event OnShowCompleted OnShowCompleted
        {
            add
            {
                onShowCompleted ??= new List<OnShowCompleted>(1);
                onShowCompleted.Add(value);
            }
            remove
            {
                if (onShowCompleted == null) return;
                for (var i = onShowCompleted.Count - 1; i >= 0; i--)
                {
                    if (onShowCompleted[i] == value) onShowCompleted.RemoveAt(i);
                }
            }
        }

        internal void RaiseOnShowCompleted()
        {
            if (onShowCompleted == null) return;
            var count = onShowCompleted.Count;
            for (var i = 0; i < count; i++)
            {
                try
                {
                    onShowCompleted[i]?.Invoke();
                }
                catch (Exception e)
                {
                    ErrorHandle.LogException(e, "Callback OnShowCompleted Error");
                }
            }
        }

        private List<OnHide> onHide;

        public event OnHide OnHide
        {
            add
            {
                onHide ??= new List<OnHide>(1);
                onHide.Add(value);
            }
            remove
            {
                if (onHide == null) return;
                for (var i = onHide.Count - 1; i >= 0; i--)
                {
                    if (onHide[i] == value) onHide.RemoveAt(i);
                }
            }
        }

        internal bool RaiseOnHide(ICommandReleaseHandle handle)
        {
            if (onHide == null) return false;
            var result = false;
            var count = onHide.Count;
            for (var i = 0; i < count; i++)
            {
                try
                {
                    if (onHide[i] == null) continue;
                    onHide[i].Invoke(handle);
                    result = true;
                }
                catch (Exception e)
                {
                    ErrorHandle.LogException(e, "Callback OnShowCompleted Error");
                }
            }

            return result;
        }

        private List<OnKeyBack> onKeyBack;

        public event OnKeyBack OnKeyBack
        {
            add
            {
                onKeyBack ??= new List<OnKeyBack>(1);
                onKeyBack.Add(value);
            }
            remove
            {
                if (onKeyBack == null) return;
                for (var i = onKeyBack.Count - 1; i >= 0; i--)
                {
                    if (onKeyBack[i] == value) onKeyBack.RemoveAt(i);
                }
            }
        }

        internal void RaiseOnKeyBack()
        {
            if (onKeyBack == null) return;
            var count = onKeyBack.Count;
            for (var i = 0; i < count; i++)
            {
                try
                {
                    onKeyBack[i]?.Invoke();
                }
                catch (Exception e)
                {
                    ErrorHandle.LogException(e, "Callback OnKeyBack Error");
                }
            }
        }

        private List<OnReFocus> onReFocus;

        public event OnReFocus OnReFocus
        {
            add
            {
                onReFocus ??= new List<OnReFocus>(1);
                onReFocus.Add(value);
            }
            remove
            {
                if (onReFocus == null) return;
                for (var i = onReFocus.Count - 1; i >= 0; i--)
                {
                    if (onReFocus[i] == value) onReFocus.RemoveAt(i);
                }
            }
        }

        internal void RaiseOnReFocus()
        {
            if (onReFocus == null) return;
            var count = onReFocus.Count;
            for (var i = 0; i < count; i++)
            {
                try
                {
                    onReFocus[i]?.Invoke();
                }
                catch (Exception e)
                {
                    ErrorHandle.LogException(e, "Callback OnReFocus Error");
                }
            }
        }

        public override void ClearDelegates(object target)
        {
            base.ClearDelegates(target);
            ClearDelegate(onShowCompleted, target);
            ClearDelegate(onHide, target);
            ClearDelegate(onKeyBack, target);
            ClearDelegate(onReFocus, target);
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
            return $"<b><size=11>OnActive</size></b>                       {(onActive == null ? "Event: 0" : GetDelegatesInfo(onActive))}\n" +
                   $"<b><size=11>OnActiveWithData</size></b>       {(onActiveWithData == null ? "Event: 0" : GetDelegatesInfo(onActiveWithData))}\n" +
                   $"<b><size=11>OnShowCompleted</size></b>     {(onShowCompleted == null ? "Event: 0" : GetDelegatesInfo(onShowCompleted))}\n" +
                   $"<b><size=11>OnHide</size></b>                          {(onHide == null ? "Event: 0" : GetDelegatesInfo(onHide))}\n" +
                   $"<b><size=11>OnKeyBack</size></b>                    {(onKeyBack == null ? "Event: 0" : GetDelegatesInfo(onKeyBack))}\n" +
                   $"<b><size=11>OnReFocus</size></b>                   {(onReFocus == null ? "Event: 0" : GetDelegatesInfo(onReFocus))}\n" +
                   $"<b><size=11>OnRelease</size></b>                    {(onRelease == null ? "Event: 0" : GetDelegatesInfo(onRelease))}";
        }
    }
}
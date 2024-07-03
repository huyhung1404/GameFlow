using UnityEngine;
using UnityEngine.UI;

namespace GameFlow.Component
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public abstract class GameFlowUICanvasWithAnimation : GameFlowUICanvas
    {
        private ICommandReleaseHandle releaseHandle;

        protected override void RegisterDelegates(UIElementCallbackEvent delegates)
        {
            base.RegisterDelegates(delegates);
            delegates.OnActive += OnShow;
            delegates.OnHide += OnHide;
        }

        protected override void UnregisterDelegates(UIElementCallbackEvent delegates)
        {
            base.UnregisterDelegates(delegates);
            delegates.OnActive -= OnShow;
            delegates.OnHide -= OnHide;
        }

        private void OnHide(ICommandReleaseHandle handle)
        {
            releaseHandle = handle;
            OnHide();
        }

        protected abstract void OnShow();
        protected abstract void OnHide();

        protected void OnShowCompleted()
        {
            FlowObservable.UIEvent(element.elementType).RaiseOnShowCompleted();
        }

        protected void OnHideCompleted()
        {
            if (releaseHandle == null) return;
            releaseHandle.Next();
            releaseHandle = null;
        }
    }
}
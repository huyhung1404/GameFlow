using GameFlow.Internal;
using UnityEngine;

namespace GameFlow.Component
{
    public abstract class GameFlowUIAnimation : ElementMonoBehaviours
    {
        [SerializeField, InternalDraw(DrawType.Element)] protected UIFlowElement element;
        protected UIElementCallbackEvent delegates;
        private ICommandReleaseHandle releaseHandle;

        protected virtual void Awake()
        {
            delegates = FlowObservable.UIEvent(element.GetType());
        }

        protected virtual void OnEnable()
        {
            delegates.OnActive += OnShow;
            delegates.OnHide += OnHide;
        }

        protected virtual void OnDisable()
        {
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
            delegates.RaiseOnShowCompleted();
        }

        protected void OnHideCompleted()
        {
            if (releaseHandle == null) return;
            releaseHandle.Next();
            releaseHandle = null;
        }

        internal override void SetElement(GameFlowElement value, System.Type type)
        {
            if (element != null && type != element.GetType()) return;
            if (value is not UIFlowElement uiElement) return;
            element = uiElement;
        }
    }
}
using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class ReleaseUIElementCommand : ReleaseCommand, ICommandReleaseHandle
    {
        private UIFlowElement element;
        protected override GameFlowElement baseElement { get => element; set => element = (UIFlowElement)value; }
        private readonly UIElementCallbackEvent delegates;

        public ReleaseUIElementCommand(Type elementType) : base(elementType)
        {
            delegates = FlowObservable.UIEvent(elementType);
        }

        internal override void PreUpdate()
        {
            element = UIElementsRuntimeManager.GetElement(elementType);
            if (element) return;
            ErrorHandle.LogWarning($"Element type {elementType.Name} is not exits in pool");
            OnLoadResult(false);
            isExecute = true;
        }

        protected override void HandleRelease()
        {
            if (delegates.RaiseOnHide(this)) return;
            Next();
        }

        public void Next()
        {
            ExecuteByReleaseMode();
        }

        protected override void ReleaseOnClose()
        {
            delegates.RaiseOnRelease(false);
            baseElement.reference.ReleaseHandlePrefab(baseElement.runtimeInstance, this);
        }

        protected override void NoneRelease()
        {
            delegates.RaiseOnRelease(false);
            baseElement.runtimeInstance.SetActive(false);
            OnLoadResult(true);
        }

        protected override void OnLoadResult(bool canRelease)
        {
            onCompleted?.Invoke(canRelease);
            if (canRelease)
            {
                callbackOnRelease = true;
                UIElementsRuntimeManager.RemoveElement(element);
            }

            Release();
        }

        internal override void OnRelease()
        {
            if (!callbackOnRelease) return;
            base.OnRelease();
            var topElement = UIElementsRuntimeManager.GetTopElement();
            if (topElement == null) return;
            FlowObservable.UIEvent(topElement).RaiseOnReFocus();
        }
    }
}
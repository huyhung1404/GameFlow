using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class ReleaseUserInterfaceElementCommand : ReleaseCommand, ICommandReleaseHandle
    {
        private UserInterfaceFlowElement element;
        protected override GameFlowElement baseElement { get => element; set => element = (UserInterfaceFlowElement)value; }
        private readonly UIElementCallbackEvent delegates;

        public ReleaseUserInterfaceElementCommand(Type elementType) : base(elementType)
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
            if (isRelease && element) UIElementsRuntimeManager.RemoveElement(element);
            Release();
        }
    }
}
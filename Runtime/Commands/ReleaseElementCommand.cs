using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class ReleaseElementCommand : ReleaseCommand
    {
        protected override GameFlowElement baseElement { get; set; }

        public ReleaseElementCommand(Type elementType) : base(elementType)
        {
        }

        internal override void PreUpdate()
        {
            baseElement = ElementsRuntimeManager.GetElement(elementType);
            if (baseElement) return;
            ErrorHandle.LogWarning($"Element type {elementType.Name} is not exists in pool");
            OnLoadResult(false);
            isExecute = true;
        }

        protected override void HandleRelease()
        {
            ExecuteByReleaseMode();
        }

        protected override void ReleaseOnClose()
        {
            FlowObservable.Event(elementType).RaiseOnRelease(false);
            baseElement.reference.ReleaseHandlePrefab(baseElement.runtimeInstance, this);
        }

        protected override void NoneRelease()
        {
            FlowObservable.Event(elementType).RaiseOnRelease(false);
            baseElement.runtimeInstance.SetActive(false);
            OnLoadResult(true);
        }

        protected override void OnLoadResult(bool canRelease)
        {
            onCompleted?.Invoke(canRelease);
            if (canRelease)
            {
                callbackOnRelease = true;
                ElementsRuntimeManager.RemoveElement(baseElement);
            }

            Release();
        }
    }
}
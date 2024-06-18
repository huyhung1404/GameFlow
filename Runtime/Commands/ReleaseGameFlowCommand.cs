using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class ReleaseGameFlowCommand : ReleaseCommand
    {
        protected override GameFlowElement baseElement { get; set; }

        public ReleaseGameFlowCommand(Type elementType) : base(elementType)
        {
        }

        internal override void PreUpdate()
        {
            baseElement = ElementsRuntimeManager.GetElement(elementType);
            if (baseElement != null) return;
            ErrorHandle.LogWarning($"Element type {elementType.Name} is not exits in pool");
            OnLoadResult(false);
            isExecute = true;
        }

        protected override void ReleaseOnClose()
        {
            FlowSubject.Event(elementType).RaiseOnRelease(false);
            baseElement.reference.ReleaseHandlePrefab(baseElement.runtimeInstance, this);
        }

        internal override void UnloadCompleted(bool isSuccess)
        {
            if (baseElement.releaseMode == ElementReleaseMode.RELEASE_ON_CLOSE_INCLUDE_CALLBACK)
            {
                FlowSubject.ReleaseEvent(elementType);
            }

            if (isSuccess)
            {
                baseElement.runtimeInstance = null;
                OnLoadResult(true);
                return;
            }

            OnLoadResult(false);
        }

        protected override void NoneRelease()
        {
            FlowSubject.Event(elementType).RaiseOnRelease(false);
            baseElement.runtimeInstance.SetActive(false);
            OnLoadResult(true);
        }

        protected override void OnLoadResult(bool canRelease)
        {
            onCompleted?.Invoke(canRelease);
            if (isRelease) ElementsRuntimeManager.RemoveElement(baseElement);
            Release();
        }
    }
}
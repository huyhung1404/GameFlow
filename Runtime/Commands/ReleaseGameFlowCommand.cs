using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class ReleaseGameFlowCommand : ReleaseCommand
    {
        public ReleaseGameFlowCommand(Type elementType, string id) : base(elementType, id)
        {
        }

        internal override void PreUpdate()
        {
            baseElement = ElementsRuntimeManager.RemoveElement(elementType, id);
            if (baseElement != null) return;
            ErrorHandle.LogWarning($"Element type {elementType.Name} is not exits in pool");
            OnLoadResult(false);
            isExecute = true;
        }

        protected override void ReleaseOnClose()
        {
            FlowSubject.Event(elementType, id).onRelease?.Invoke(false);
            baseElement.reference.ReleaseHandlePrefab(baseElement.runtimeInstance, this);
        }

        internal override void UnloadCompleted(bool isSuccess)
        {
            if (baseElement.releaseMode == ElementReleaseMode.RELEASE_ON_CLOSE_INCLUDE_CALLBACK)
            {
                FlowSubject.ReleaseEvent(elementType, id);
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
            FlowSubject.Event(elementType, id).onRelease?.Invoke(false);
            baseElement.runtimeInstance.SetActive(false);
            OnLoadResult(true);
        }

        protected override void OnLoadResult(bool canRelease)
        {
            onCompleted?.Invoke(canRelease);
            Release();
        }
    }
}
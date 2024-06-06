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

        protected override void NoneRelease()
        {
            FlowSubject.Event(elementType, id).onRelease?.Invoke(false);
            baseElement.runtimeInstance.SetActive(false);
        }
    }
}
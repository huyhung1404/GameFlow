using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class ReleaseElementCommand : ReleaseCommand
    {
        protected override GameFlowElement BaseElement { get; set; }

        public ReleaseElementCommand(Type elementType) : base(elementType)
        {
        }

        internal override void PreUpdate()
        {
            BaseElement = ElementsRuntimeManager.GetElement(_elementType);
            if (BaseElement) return;
            ErrorHandle.LogWarning($"Element type {_elementType.Name} is not exists in pool");
            OnLoadResult(false);
            _isExecute = true;
        }

        protected override void HandleRelease()
        {
            ExecuteByReleaseMode();
        }

        protected override void ReleaseOnClose()
        {
            FlowObservable.Event(_elementType).RaiseOnRelease(false);
            BaseElement.Reference.ReleaseHandlePrefab(BaseElement.RuntimeInstance, this);
        }

        protected override void NoneRelease()
        {
            FlowObservable.Event(_elementType).RaiseOnRelease(false);
            BaseElement.RuntimeInstance.SetActive(false);
            OnLoadResult(true);
        }

        protected override void OnLoadResult(bool canRelease)
        {
            OnCompleted?.Invoke(canRelease);
            if (canRelease)
            {
                _callbackOnRelease = true;
                ElementsRuntimeManager.RemoveElement(BaseElement);
            }

            Release();
        }
    }
}
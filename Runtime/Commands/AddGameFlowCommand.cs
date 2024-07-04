using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class AddGameFlowCommand : AddCommand
    {
        protected override GameFlowElement baseElement { get; set; }

        public AddGameFlowCommand(Type elementType) : base(elementType)
        {
        }

        protected override void PreActive()
        {
        }

        protected override void ReActiveElement()
        {
            FlowObservable.Event(elementType).RaiseOnRelease(true);
            baseElement.runtimeInstance.SetActive(false);
            baseElement.runtimeInstance.SetActive(true);
            callbackOnRelease = true;
            Release();
        }

        protected override void ActiveElement()
        {
            baseElement.runtimeInstance.SetActive(true);
            ElementsRuntimeManager.AddElement(baseElement);
            callbackOnRelease = true;
            Release();
        }
    }
}
using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class AddGameFlowCommand : AddCommand
    {
        protected override GameFlowElement BaseElement { get; set; }

        public AddGameFlowCommand(Type elementType) : base(elementType)
        {
        }

        protected override void ReActiveElement()
        {
            FlowObservable.Event(_elementType).RaiseOnRelease(true);
            BaseElement.RuntimeInstance.SetActive(false);
            BaseElement.RuntimeInstance.SetActive(true);
            _callbackOnRelease = true;
            Release();
        }

        protected override void ActiveElement()
        {
            BaseElement.RuntimeInstance.SetActive(true);
            ElementsRuntimeManager.AddElement(BaseElement);
            _callbackOnRelease = true;
            Release();
        }
    }
}
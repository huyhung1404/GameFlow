using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class AddUICommand : AddCommand
    {
        private UIFlowElement _element;
        protected override GameFlowElement BaseElement { get => _element; set => _element = (UIFlowElement)value; }

        internal AddUICommand(Type elementType) : base(elementType)
        {
        }

        protected override void ReActiveElement()
        {
            FlowObservable.Event(_elementType).RaiseOnRelease(true);
            BaseElement.RuntimeInstance.SetActive(false);
            UIElementsRuntimeManager.RemoveElement(_element);
            BaseElement.RuntimeInstance.SetActive(true);
            UIElementsRuntimeManager.AddUserInterfaceElement(_element);
            _callbackOnRelease = true;
            Release();
        }

        protected override void ActiveElement()
        {
            BaseElement.RuntimeInstance.SetActive(true);
            UIElementsRuntimeManager.AddUserInterfaceElement(_element);
            _callbackOnRelease = true;
            Release();
        }
    }
}
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
            BaseElement.EnsureCallbackEvent();
            BaseElement.CallbackEvent.RaiseOnRelease(true);
            BaseElement.RuntimeInstance.SetActive(false);
            Context.UIElementsRuntime.RemoveElement(_element);
            BaseElement.RuntimeInstance.SetActive(true);
            Context.UIElementsRuntime.AddUserInterfaceElement(_element);
            _callbackOnRelease = true;
            Release();
        }

        protected override void ActiveElement()
        {
            BaseElement.EnsureCallbackEvent();
            BaseElement.RuntimeInstance.SetActive(true);
            Context.UIElementsRuntime.AddUserInterfaceElement(_element);
            _callbackOnRelease = true;
            Release();
        }
    }
}

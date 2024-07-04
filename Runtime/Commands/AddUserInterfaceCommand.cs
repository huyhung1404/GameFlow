using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class AddUserInterfaceCommand : AddCommand
    {
        private UserInterfaceFlowElement element;
        protected override GameFlowElement baseElement { get => element; set => element = (UserInterfaceFlowElement)value; }

        internal AddUserInterfaceCommand(Type elementType) : base(elementType)
        {
        }

        protected override void ReActiveElement()
        {
            FlowObservable.Event(elementType).RaiseOnRelease(true);
            UIElementsRuntimeManager.GetElement(elementType);
            baseElement.runtimeInstance.SetActive(false);
            baseElement.runtimeInstance.SetActive(true);
            UIElementsRuntimeManager.AddUserInterfaceElement(element);
            callbackOnRelease = true;
            Release();
        }

        protected override void ActiveElement()
        {
            baseElement.runtimeInstance.SetActive(true);
            UIElementsRuntimeManager.AddUserInterfaceElement(element);
            callbackOnRelease = true;
            Release();
        }
    }
}
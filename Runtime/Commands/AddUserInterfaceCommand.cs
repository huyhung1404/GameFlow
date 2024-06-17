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
            FlowSubject.Event(elementType).RaiseOnRelease(true);
            UserInterfaceElementsRuntimeManager.ReleaseElement(elementType);
            baseElement.runtimeInstance.SetActive(false);
            baseElement.runtimeInstance.SetActive(true);
            UserInterfaceElementsRuntimeManager.AddUserInterfaceElement(element);
            callbackOnRelease = true;
        }

        protected override void ActiveElement()
        {
            baseElement.runtimeInstance.SetActive(true);
            UserInterfaceElementsRuntimeManager.AddUserInterfaceElement(element);
            callbackOnRelease = true;
        }
    }
}
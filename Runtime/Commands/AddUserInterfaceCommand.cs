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
        }

        protected override void ActiveElement()
        {
            baseElement.runtimeInstance.SetActive(true);
            UserInterfaceElementsRuntimeManager.AddUserInterfaceElement(element);
            callbackOnRelease = true;
            OnLoadResult(baseElement.runtimeInstance);
        }
    }
}
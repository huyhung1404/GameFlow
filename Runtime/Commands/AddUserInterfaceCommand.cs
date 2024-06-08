using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class AddUserInterfaceCommand : AddCommand
    {
        private UserInterfaceFlowElement element;
        protected override GameFlowElement baseElement { get => element; set => element = (UserInterfaceFlowElement)value; }
        private bool callbackOnRelease;

        internal AddUserInterfaceCommand(Type elementType) : base(elementType)
        {
            callbackOnRelease = false;
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

        internal override void OnRelease()
        {
            if (!callbackOnRelease) return;
            if (ReferenceEquals(sendData, null))
            {
                FlowSubject.Event(elementType).RaiseOnActive();
                return;
            }

            FlowSubject.Event(elementType).RaiseOnActiveWithData(sendData);
        }
    }
}
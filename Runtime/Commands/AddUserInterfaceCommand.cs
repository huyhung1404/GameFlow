using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class AddUserInterfaceCommand : AddCommand
    {
        private UserInterfaceFlowElement element;
        protected override GameFlowElement baseElement { get => element; set => element = (UserInterfaceFlowElement)value; }
        private bool callbackOnRelease;
        protected int elementIndex;

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
            elementIndex = UserInterfaceElementsRuntimeManager.AddUserInterfaceElement(element);
            callbackOnRelease = true;
            OnLoadResult(baseElement.runtimeInstance);
        }

        internal override void OnRelease()
        {
            if (!callbackOnRelease) return;
            var callback = FlowSubject.UIEvent(elementType);
            callback.RaiseOnSetUpCanvas(elementIndex * GameFlowRuntimeController.Manager().sortingOrderOffset);
            if (ReferenceEquals(sendData, null))
            {
                callback.RaiseOnActive();
                return;
            }

            callback.RaiseOnActiveWithData(sendData);
        }
    }
}
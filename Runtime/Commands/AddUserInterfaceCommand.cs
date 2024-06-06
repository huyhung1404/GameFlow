using System;

namespace GameFlow
{
    public class AddUserInterfaceCommand : AddCommand
    {
        private UserInterfaceFlowElement element;
        protected override GameFlowElement baseElement { get => element; set => element = (UserInterfaceFlowElement)value; }
        private bool callbackOnRelease;

        internal AddUserInterfaceCommand(Type elementType, string id) : base(elementType, id)
        {
            callbackOnRelease = false;
        }

        protected override void ReActiveElement()
        {
        }

        protected override void ActiveElement()
        {
        }

        internal override void OnRelease()
        {
            if (!callbackOnRelease) return;
            if (ReferenceEquals(sendData, null))
            {
                FlowSubject.Event(elementType, id).RaiseOnActive();
                return;
            }

            FlowSubject.Event(elementType, id).RaiseOnActiveWithData(sendData);
        }
    }
}
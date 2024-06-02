using System;

namespace GameFlow
{
    public class AddUserInterfaceCommand : AddCommand
    {
        private UserInterfaceFlowElement element;

        protected override GameFlowElement baseElement
        {
            get => element;
            set => element = (UserInterfaceFlowElement)value;
        }

        internal AddUserInterfaceCommand(Type elementType, string id) : base(elementType, id)
        {
        }

        protected override void ReActiveElement()
        {
        }

        protected override void ActiveElement()
        {
        }
    }
}
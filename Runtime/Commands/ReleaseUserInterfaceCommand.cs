using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class ReleaseUserInterfaceCommand : ReleaseCommand, ICommandReleaseHandle
    {
        private UserInterfaceFlowElement element;
        protected override GameFlowElement baseElement { get => element; set => element = (UserInterfaceFlowElement)value; }

        public ReleaseUserInterfaceCommand(Type elementType) : base(elementType)
        {
            element = UIElementsRuntimeManager.GetElement(elementType);
        }

        internal override void PreUpdate()
        {
        }

        protected override void ReleaseOnClose()
        {
        }

        internal override void UnloadCompleted(bool isSuccess)
        {
        }

        protected override void NoneRelease()
        {
        }

        protected override void OnLoadResult(bool canRelease)
        {
        }

        public void Next()
        {
        }
    }
}
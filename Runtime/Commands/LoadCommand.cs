using System;

namespace GameFlow
{
    public class LoadCommand : AddUserInterfaceCommand
    {
        internal bool autoActive;

        internal LoadCommand(Type elementType) : base(elementType)
        {
            activeHandle = new ReferenceActiveHandleForLoadCommand(this);
            activeHandle.OnLoadResult += OnActiveHandleLoadResult;
            autoActive = true;
        }

        private void OnActiveHandleLoadResult(ActiveHandleStatus status)
        {
            if (status != ActiveHandleStatus.Succeeded) return;
            if (!autoActive) return;
            activeHandle.ActiveScene();
        }
    }
}
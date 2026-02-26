using System;

namespace GameFlow
{
    public class LoadCommand : AddUICommand
    {
        internal bool AutoActive;

        internal LoadCommand(Type elementType) : base(elementType)
        {
            ActiveHandle = new ReferenceActiveHandleForLoadCommand(this);
            ActiveHandle.OnLoadResult += OnActiveHandleLoadResult;
            AutoActive = true;
        }

        private void OnActiveHandleLoadResult(ActiveHandleStatus status)
        {
            if (status != ActiveHandleStatus.Succeeded) return;
            if (!AutoActive) return;
            ActiveHandle.ActiveScene();
        }

        internal override string GetFullInfo()
        {
            return base.GetFullInfo() + $"\n<b><size=11>autoActive:</size></b> {AutoActive}";
        }
    }
}
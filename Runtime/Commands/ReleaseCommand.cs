using System;
using GameFlow.Internal;

namespace GameFlow
{
    public delegate void OnReleaseCommandCompleted(bool isRelease);

    public abstract class ReleaseCommand : Command
    {
        protected bool isExecute;
        internal OnReleaseCommandCompleted onCompleted;
        protected abstract GameFlowElement baseElement { get; set; }

        internal ReleaseCommand(Type elementType) : base(elementType)
        {
            isExecute = false;
        }

        internal override void Update()
        {
            if (isExecute) return;
            isExecute = Execute();
        }

        private bool Execute()
        {
            try
            {
                var reference = baseElement.reference;
                if (!reference.IsReady()) return false;
                switch (baseElement.releaseMode)
                {
                    default:
                    case ElementReleaseMode.RELEASE_ON_CLOSE:
                    case ElementReleaseMode.RELEASE_ON_CLOSE_INCLUDE_CALLBACK:
                        ReleaseOnClose();
                        break;
                    case ElementReleaseMode.NONE_RELEASE:
                        NoneRelease();
                        break;
                }

                return true;
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, $"Release Command Error: {elementType.Name}");
                OnLoadResult(false);
                return true;
            }
        }

        protected abstract void ReleaseOnClose();

        internal abstract void UnloadCompleted(bool isSuccess);

        protected abstract void NoneRelease();

        protected abstract void OnLoadResult(bool canRelease);
    }
}
using System;
using GameFlow.Internal;

namespace GameFlow
{
    public delegate void OnReleaseCommandCompleted(bool isRelease);

    public abstract class ReleaseCommand : Command, IReleaseCompleted
    {
        protected bool _isExecute;
        protected bool _callbackOnRelease;
        internal OnReleaseCommandCompleted OnCompleted;
        internal bool IgnoreAnimationHide;
        protected abstract GameFlowElement BaseElement { get; set; }

        internal ReleaseCommand(Type elementType) : base(elementType)
        {
            _isExecute = false;
        }

        internal override void Update()
        {
            if (_isExecute) return;
            _isExecute = Execute();
        }

        private bool Execute()
        {
            try
            {
                var reference = BaseElement.Reference;
                if (!reference.IsReady()) return false;
                HandleRelease();
                return true;
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, $"Release Command Error: {_elementType.Name}");
                OnLoadResult(false);
                return true;
            }
        }

        protected abstract void HandleRelease();

        protected void ExecuteByReleaseMode()
        {
            switch (BaseElement.ReleaseMode)
            {
                default:
                case ElementReleaseMode.ReleaseOnClose:
                case ElementReleaseMode.ReleaseOnCloseIncludeCallback:
                    ReleaseOnClose();
                    break;
                case ElementReleaseMode.NoneRelease:
                    NoneRelease();
                    break;
            }
        }

        void IReleaseCompleted.UnloadCompleted(bool isSuccess)
        {
            if (isSuccess)
            {
                BaseElement.RuntimeInstance = null;
                OnLoadResult(true);
                return;
            }

            OnLoadResult(false);
        }

        protected abstract void ReleaseOnClose();
        protected abstract void NoneRelease();
        protected abstract void OnLoadResult(bool canRelease);

        internal override void OnRelease()
        {
            if (!_callbackOnRelease) return;
            if (BaseElement.ReleaseMode == ElementReleaseMode.ReleaseOnCloseIncludeCallback) FlowObservable.ReleaseEvent(_elementType);
        }

        internal override string GetFullInfo()
        {
            return $@"<b><size=11>isRelease:</size></b> {IsRelease}
<b><size=11>onCompleted:</size></b> {(OnCompleted != null ? $"{OnCompleted.Target}.{OnCompleted.Method.Name}" : "None")}
<b><size=11>isExecute:</size></b> {_isExecute}
<b><size=11>isUserInterface:</size></b> {this is ReleaseUIElementCommand}";
        }
    }
}
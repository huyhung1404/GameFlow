using System;

namespace GameFlow
{
    public class LoadCommand : AddUICommand
    {
        internal bool AutoActive;
        private bool _releasedOldElements;
        private bool _waitingForRelease;

        internal LoadCommand(Type elementType) : base(elementType)
        {
            ActiveHandle = new ReferenceActiveHandle(this);
            ActiveHandle.OnLoadResult += OnActiveHandleLoadResult;
            AutoActive = true;
        }

        private void OnActiveHandleLoadResult(ActiveHandleStatus status)
        {
            if (status != ActiveHandleStatus.Succeeded) return;
            if (!AutoActive) return;
            ActiveHandle.ActiveScene();
        }

        internal override void Update()
        {
            if (!_releasedOldElements)
            {
                _releasedOldElements = true;
                if (Context.UIElementsRuntime.ElementsRuntime.Count > 0)
                {
                    _waitingForRelease = true;
                    ReleaseNextElement();
                    return;
                }
            }

            if (_waitingForRelease) return;
            base.Update();
        }

        private void ReleaseNextElement()
        {
            var elements = Context.UIElementsRuntime.ElementsRuntime;
            if (elements.Count == 0)
            {
                _waitingForRelease = false;
                return;
            }

            var releaseCommand = new ReleaseUIElementCommand(elements[elements.Count - 1].ElementType);
            releaseCommand.IgnoreAnimationHide = true;
            Context.RuntimeController.SetPriorityCommand(releaseCommand, ReleaseNextElement);
        }

        internal override string GetFullInfo()
        {
            return base.GetFullInfo() + $"\n<b><size=11>autoActive:</size></b> {AutoActive}";
        }
    }

    internal sealed class LoadCommand<TData> : LoadCommand
    {
        private readonly TData _data;

        internal LoadCommand(Type elementType, TData data) : base(elementType)
        {
            _data = data;
        }

        internal override void RaiseActiveData(ElementCallbackEvent delegates) => delegates.RaiseOnActiveWithData(_data);
#if UNITY_EDITOR
        internal override string GetActiveDataInfo() => _data?.ToString() ?? "null";
#endif
    }
}

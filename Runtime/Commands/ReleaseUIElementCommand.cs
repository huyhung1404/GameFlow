using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class ReleaseUIElementCommand : ReleaseCommand, ICommandReleaseHandle
    {
        private UIFlowElement _element;
        protected override GameFlowElement BaseElement { get => _element; set => _element = (UIFlowElement)value; }
        private readonly UIElementCallbackEvent _delegates;

        public ReleaseUIElementCommand(Type elementType) : base(elementType)
        {
            _delegates = FlowObservable.UIEvent(elementType);
        }

        internal override void PreUpdate()
        {
            _element = UIElementsRuntimeManager.GetElement(_elementType);
            if (_element) return;
            ErrorHandle.LogWarning($"Element type {_elementType.Name} is not exists in pool");
            OnLoadResult(false);
            _isExecute = true;
        }

        protected override void HandleRelease()
        {
            if (!IgnoreAnimationHide && _delegates.RaiseOnHide(this))
            {
                return;
            }

            Next();
        }

        public void Next()
        {
            ExecuteByReleaseMode();
        }

        protected override void ReleaseOnClose()
        {
            _delegates.RaiseOnRelease(false);
            BaseElement.Reference.ReleaseHandlePrefab(BaseElement.RuntimeInstance, this);
        }

        protected override void NoneRelease()
        {
            _delegates.RaiseOnRelease(false);
            BaseElement.RuntimeInstance.SetActive(false);
            OnLoadResult(true);
        }

        protected override void OnLoadResult(bool canRelease)
        {
            OnCompleted?.Invoke(canRelease);
            if (canRelease)
            {
                _callbackOnRelease = true;
                UIElementsRuntimeManager.RemoveElement(_element);
            }

            Release();
        }

        internal override void OnRelease()
        {
            if (!_callbackOnRelease) return;
            base.OnRelease();
            var topElement = UIElementsRuntimeManager.GetTopElement();
            if (topElement == null) return;
            FlowObservable.UIEvent(topElement).RaiseOnReFocus();
        }
    }
}
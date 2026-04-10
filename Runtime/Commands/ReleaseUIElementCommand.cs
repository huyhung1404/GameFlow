using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class ReleaseUIElementCommand : ReleaseCommand, ICommandReleaseHandle
    {
        private UIFlowElement _element;
        protected override GameFlowElement BaseElement { get => _element; set => _element = (UIFlowElement)value; }
        private UIElementCallbackEvent _delegates;

        public ReleaseUIElementCommand(Type elementType) : base(elementType)
        {
        }

        internal override void PreUpdate()
        {
            _element = Context.UIElementsRuntime.GetElement(_elementType);
            if (!_element)
            {
                ErrorHandle.LogWarning($"Element type {_elementType.Name} is not exists in pool");
                OnReleaseResult(false);
                _isExecute = true;
                return;
            }

            _delegates = (UIElementCallbackEvent)_element.CallbackEvent;
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
            OnReleaseResult(true);
        }

        protected override void OnReleaseResult(bool canRelease)
        {
            OnCompleted?.Invoke(canRelease);
            if (canRelease)
            {
                _callbackOnRelease = true;
                Context.UIElementsRuntime.RemoveElement(_element);
            }

            Release();
        }

        internal override void OnRelease()
        {
            if (!_callbackOnRelease) return;
            base.OnRelease();
            var elements = Context.UIElementsRuntime.ElementsRuntime;
            if (elements.Count == 0) return;
            var topElement = elements[elements.Count - 1];
            if (topElement.CallbackEvent is UIElementCallbackEvent uiEvent)
                uiEvent.RaiseOnReFocus();
        }
    }
}

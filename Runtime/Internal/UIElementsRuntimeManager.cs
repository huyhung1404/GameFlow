using System;
using System.Collections.Generic;

namespace GameFlow.Internal
{
    internal class UIElementsRuntimeManager
    {
        internal List<UIFlowElement> ElementsRuntime { get; }
        private readonly GameFlowContext _context;

        internal UIElementsRuntimeManager(GameFlowContext context)
        {
            _context = context;
            ElementsRuntime = new List<UIFlowElement>();
        }

        internal void AddUserInterfaceElement(UIFlowElement userInterfaceFlowElement)
        {
            userInterfaceFlowElement.CurrentSortingOrder = GetSortingOrder();
            ElementsRuntime.Add(userInterfaceFlowElement);
        }

        internal Type GetTopElement()
        {
            var elementCount = ElementsRuntime.Count;
            return elementCount == 0 ? null : ElementsRuntime[elementCount - 1].ElementType;
        }

        internal int GetSortingOrder()
        {
            var elementCount = ElementsRuntime.Count;
            if (elementCount == 0) return 0;
            return ElementsRuntime[elementCount - 1].CurrentSortingOrder + _context.Manager.SortingOrderOffset;
        }

        internal UIFlowElement GetElement(Type type)
        {
            for (var i = ElementsRuntime.Count - 1; i >= 0; i--)
            {
                if (ElementsRuntime[i].ElementType != type) continue;
                return ElementsRuntime[i];
            }

            return null;
        }

        internal void RemoveElement(UIFlowElement element)
        {
            ElementsRuntime.Remove(element);
        }

        internal void ReleaseAllElement(Action onReleaseCompleted)
        {
            var elementCount = ElementsRuntime.Count;
            if (elementCount == 0)
            {
                onReleaseCompleted?.Invoke();
                return;
            }

            var releaseCount = new ReleaseCount(onReleaseCompleted, elementCount);
            for (var i = ElementsRuntime.Count - 1; i >= 0; i--)
            {
                ReleaseElement(ElementsRuntime[i], releaseCount);
            }
        }

        private void ReleaseElement(UIFlowElement element, ReleaseCount releaseCount)
        {
            element.CallbackEvent?.RaiseOnRelease(true);
            switch (element.ReleaseMode)
            {
                default:
                case ElementReleaseMode.ReleaseOnClose:
                case ElementReleaseMode.ReleaseOnCloseIncludeCallback:
                    element.Reference.ReleaseHandlePrefab(element.RuntimeInstance, releaseCount);
                    break;
                case ElementReleaseMode.NoneRelease:
                    element.RuntimeInstance.SetActive(false);
                    releaseCount.Count();
                    break;
            }
        }

        internal void OnKeyBack()
        {
            var elementCount = ElementsRuntime.Count;
            if (elementCount == 0) return;
            var topElement = ElementsRuntime[elementCount - 1];
            if (topElement.CallbackEvent is UIElementCallbackEvent uiEvent)
                uiEvent.RaiseOnKeyBack();
        }

        internal class ReleaseCount : IReleaseCompleted
        {
            private readonly Action _onCompleted;
            private readonly int _totalCount;
            private int _current;

            internal ReleaseCount(Action onCompleted, int totalCount)
            {
                _onCompleted = onCompleted;
                _totalCount = totalCount;
            }

            void IReleaseCompleted.UnloadCompleted(bool isSuccess)
            {
                Count();
            }

            internal void Count()
            {
                if (++_current == _totalCount) _onCompleted.Invoke();
            }
        }
    }
}

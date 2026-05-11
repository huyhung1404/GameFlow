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

        internal void OnKeyBack()
        {
            var elementCount = ElementsRuntime.Count;
            if (elementCount == 0) return;
            var topElement = ElementsRuntime[elementCount - 1];
            if (topElement.CallbackEvent is UIElementCallbackEvent uiEvent)
                uiEvent.RaiseOnKeyBack();
        }
    }
}

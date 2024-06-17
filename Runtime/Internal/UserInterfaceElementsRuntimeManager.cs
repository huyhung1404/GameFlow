using System;
using System.Collections.Generic;

namespace GameFlow.Internal
{
    public static class UserInterfaceElementsRuntimeManager
    {
        private static readonly List<UserInterfaceFlowElement> elementsRuntime;

        static UserInterfaceElementsRuntimeManager()
        {
            elementsRuntime = new List<UserInterfaceFlowElement>();
        }

        internal static void AddUserInterfaceElement(UserInterfaceFlowElement userInterfaceFlowElement)
        {
            userInterfaceFlowElement.currentSortingOrder = GetSortingOrder();
            elementsRuntime.Add(userInterfaceFlowElement);
        }

        private static int GetSortingOrder()
        {
            var elementCount = elementsRuntime.Count;
            if (elementCount == 0) return 0;
            return elementsRuntime[elementCount - 1].currentSortingOrder + GameFlowRuntimeController.Manager().sortingOrderOffset;
        }

        internal static UserInterfaceFlowElement ReleaseElement(Type type)
        {
            for (var i = elementsRuntime.Count - 1; i >= 0; i--)
            {
                if (elementsRuntime[i].elementType != type) continue;
                var element = elementsRuntime[i];
                elementsRuntime.RemoveAt(i);
                return element;
            }

            return null;
        }
    }
}
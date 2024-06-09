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
            userInterfaceFlowElement.currentSortingOrder = elementsRuntime.Count == 0
                ? 0
                : userInterfaceFlowElement.currentSortingOrder = elementsRuntime[elementsRuntime.Count - 1].currentSortingOrder + GameFlowRuntimeController.Manager().sortingOrderOffset;
            elementsRuntime.Add(userInterfaceFlowElement);
        }
    }
}
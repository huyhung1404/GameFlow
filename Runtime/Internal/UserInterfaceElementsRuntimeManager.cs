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
            elementsRuntime.Add(userInterfaceFlowElement);
        }
    }
}
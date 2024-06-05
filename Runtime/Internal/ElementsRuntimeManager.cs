using System;
using System.Collections.Generic;

namespace GameFlow.Internal
{
    internal static class ElementsRuntimeManager
    {
        private static readonly List<GameFlowElement> elementsRuntime;

        static ElementsRuntimeManager()
        {
            elementsRuntime = new List<GameFlowElement>();
        }

        internal static void AddElement(GameFlowElement element)
        {
            elementsRuntime.Add(element);
        }

        internal static void RemoveElement(Type type, string id)
        {
                        
        }
    }
}
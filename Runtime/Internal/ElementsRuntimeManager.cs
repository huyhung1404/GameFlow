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

        internal static GameFlowElement RemoveElement(Type type, string id)
        {
            for (var i = elementsRuntime.Count - 1; i >= 0; i--)
            {
                if (type != elementsRuntime[i].elementType) continue;
                if (!Utility.FlowIDEquals(id, elementsRuntime[i].instanceID)) continue;
                var result = elementsRuntime[i];
                elementsRuntime.RemoveAt(i);
                return result;
            }

            return null;
        }
    }
}
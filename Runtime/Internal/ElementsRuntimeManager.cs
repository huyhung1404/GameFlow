using System;
using System.Collections.Generic;

namespace GameFlow.Internal
{
    internal static class ElementsRuntimeManager
    {
        internal static List<GameFlowElement> ElementsRuntime { get; }

        static ElementsRuntimeManager()
        {
            ElementsRuntime = new List<GameFlowElement>();
        }

        internal static void AddElement(GameFlowElement element)
        {
            ElementsRuntime.Add(element);
        }

        internal static GameFlowElement GetElement(Type type)
        {
            for (var i = ElementsRuntime.Count - 1; i >= 0; i--)
            {
                if (type != ElementsRuntime[i].ElementType) continue;
                return ElementsRuntime[i];
            }

            return null;
        }

        internal static void RemoveElement(GameFlowElement element)
        {
            ElementsRuntime.Remove(element);
        }
    }
}
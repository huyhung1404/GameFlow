using System;
using System.Collections.Generic;

namespace GameFlow.Internal
{
    internal static class ElementsRuntimeManager
    {
#if UNITY_EDITOR
        internal static List<GameFlowElement> elementsRuntime { get; }
#else
        private static readonly List<GameFlowElement> elementsRuntime;
#endif

        static ElementsRuntimeManager()
        {
            elementsRuntime = new List<GameFlowElement>();
        }

        internal static void AddElement(GameFlowElement element)
        {
            elementsRuntime.Add(element);
        }

        internal static GameFlowElement GetElement(Type type)
        {
            for (var i = elementsRuntime.Count - 1; i >= 0; i--)
            {
                if (type != elementsRuntime[i].elementType) continue;
                return elementsRuntime[i];
            }

            return null;
        }

        internal static void RemoveElement(GameFlowElement element)
        {
            elementsRuntime.Remove(element);
        }
    }
}
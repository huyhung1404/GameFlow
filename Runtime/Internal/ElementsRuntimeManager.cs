using System;
using System.Collections.Generic;

namespace GameFlow.Internal
{
    internal class ElementsRuntimeManager
    {
        internal List<GameFlowElement> ElementsRuntime { get; }

        internal ElementsRuntimeManager()
        {
            ElementsRuntime = new List<GameFlowElement>();
        }

        internal void AddElement(GameFlowElement element)
        {
            ElementsRuntime.Add(element);
        }

        internal GameFlowElement GetElement(Type type)
        {
            for (var i = ElementsRuntime.Count - 1; i >= 0; i--)
            {
                if (type != ElementsRuntime[i].ElementType) continue;
                return ElementsRuntime[i];
            }

            return null;
        }

        internal void RemoveElement(GameFlowElement element)
        {
            ElementsRuntime.Remove(element);
        }
    }
}

using GameFlow.Component;
using UnityEngine;

namespace GameFlow.Internal
{
    public abstract class CloneElement
    {
        internal abstract GameFlowElement CloneElementInstance();

        internal GameObject RuntimeInstance() => CloneElementInstance().runtimeInstance;

        internal void ReplaceElement()
        {
            var type = CloneElementInstance().elementType;
            foreach (var child in RuntimeInstance().GetComponentsInChildren<ElementMonoBehaviours>(true))
            {
                child.SetElement(CloneElementInstance(), type);
            }
        }

        internal abstract void ActiveElement();
    }
}
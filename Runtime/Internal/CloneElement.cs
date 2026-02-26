using GameFlow.Component;
using UnityEngine;

namespace GameFlow.Internal
{
    public abstract class CloneElement
    {
        internal abstract GameFlowElement CloneElementInstance();

        internal GameObject RuntimeInstance() => CloneElementInstance().RuntimeInstance;

        internal void ReplaceElement()
        {
            var type = CloneElementInstance().ElementType;
            foreach (var child in RuntimeInstance().GetComponentsInChildren<ElementMonoBehaviours>(true))
            {
                child.SetElement(CloneElementInstance(), type);
            }
        }

        internal abstract void ActiveElement();
    }
}
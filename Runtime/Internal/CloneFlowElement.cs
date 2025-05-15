using UnityEngine;

namespace GameFlow.Internal
{
    public class CloneFlowElement : CloneElement
    {
        private readonly GameFlowElement cloneElement;

        public CloneFlowElement(GameFlowElement baseElement)
        {
            cloneElement = (GameFlowElement)ScriptableObject.CreateInstance(baseElement.elementType);
            cloneElement.runtimeInstance = null;
            cloneElement.elementType = baseElement.elementType;
            cloneElement.includeInBuild = true;
            cloneElement.reference = baseElement.reference;
            cloneElement.releaseMode = ElementReleaseMode.RELEASE_ON_CLOSE;
            cloneElement.activeMode = baseElement.activeMode;
        }

        internal override GameFlowElement CloneElementInstance() => cloneElement;

        internal override void ActiveElement()
        {
            cloneElement.runtimeInstance.SetActive(true);
            ElementsRuntimeManager.AddElement(cloneElement);
        }
    }
}
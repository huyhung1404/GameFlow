using UnityEngine;

namespace GameFlow.Internal
{
    internal class UICloneElement : CloneElement
    {
        private readonly UIFlowElement cloneElement;

        public UICloneElement(UIFlowElement baseElement)
        {
            cloneElement = (UIFlowElement)ScriptableObject.CreateInstance(baseElement.elementType);
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
            UIElementsRuntimeManager.AddUserInterfaceElement(cloneElement);
        }
    }
}
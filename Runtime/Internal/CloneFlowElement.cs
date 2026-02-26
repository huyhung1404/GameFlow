using UnityEngine;

namespace GameFlow.Internal
{
    public class CloneFlowElement : CloneElement
    {
        private readonly GameFlowElement _cloneElement;

        public CloneFlowElement(GameFlowElement baseElement)
        {
            _cloneElement = (GameFlowElement)ScriptableObject.CreateInstance(baseElement.ElementType);
            _cloneElement.RuntimeInstance = null;
            _cloneElement.ElementType = baseElement.ElementType;
            _cloneElement.IncludeInBuild = true;
            _cloneElement.Reference = baseElement.Reference;
            _cloneElement.ReleaseMode = ElementReleaseMode.ReleaseOnClose;
            _cloneElement.ActiveMode = baseElement.ActiveMode;
        }

        internal override GameFlowElement CloneElementInstance() => _cloneElement;

        internal override void ActiveElement()
        {
            _cloneElement.RuntimeInstance.SetActive(true);
            ElementsRuntimeManager.AddElement(_cloneElement);
        }
    }
}
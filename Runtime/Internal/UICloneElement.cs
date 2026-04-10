using UnityEngine;

namespace GameFlow.Internal
{
    internal class UICloneElement : CloneElement
    {
        private readonly UIFlowElement _cloneElement;

        public UICloneElement(UIFlowElement baseElement)
        {
            _cloneElement = (UIFlowElement)ScriptableObject.CreateInstance(baseElement.ElementType);
            _cloneElement.RuntimeInstance = null;
            _cloneElement.ElementType = baseElement.ElementType;
            _cloneElement.IncludeInBuild = true;
            _cloneElement.Reference = baseElement.Reference;
            _cloneElement.ReleaseMode = ElementReleaseMode.ReleaseOnClose;
            _cloneElement.ActiveMode = baseElement.ActiveMode;
            _cloneElement.CallbackEvent = new UIElementCallbackEvent();
        }

        internal override GameFlowElement CloneElementInstance() => _cloneElement;

        internal override void ActiveElement(GameFlowContext context)
        {
            _cloneElement.RuntimeInstance.SetActive(true);
            context.UIElementsRuntime.AddUserInterfaceElement(_cloneElement);
        }
    }
}

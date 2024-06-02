using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class AddGameFlowCommand : AddCommand
    {
        protected override GameFlowElement baseElement { get; set; }

        public AddGameFlowCommand(Type elementType, string id) : base(elementType, id)
        {
        }

        protected override void CloseElement()
        {
            baseElement.runtimeInstance.SetActive(false);
        }

        protected override void ActiveElement()
        {
            baseElement.runtimeInstance.SetActive(true);
            ElementsRuntimeManager.AddElement(baseElement);
            OnLoadResult(baseElement.runtimeInstance);
        }
    }
}
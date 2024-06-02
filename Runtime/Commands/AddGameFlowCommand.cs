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

        protected override void ReActiveElement()
        {
            baseElement.runtimeInstance.SetActive(false);
            GameFlowEvent.OnClose(elementType, id, true);
            baseElement.runtimeInstance.SetActive(true);
            GameFlowEvent.OnActive(elementType, id, sendData);
            OnLoadResult(baseElement.runtimeInstance);
        }

        protected override void ActiveElement()
        {
            baseElement.runtimeInstance.SetActive(true);
            ElementsRuntimeManager.AddElement(baseElement);
            GameFlowEvent.OnActive(elementType, id, sendData);
            OnLoadResult(baseElement.runtimeInstance);
        }
    }
}
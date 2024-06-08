using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class AddGameFlowCommand : AddCommand
    {
        protected override GameFlowElement baseElement { get; set; }
        private bool callbackOnRelease;

        public AddGameFlowCommand(Type elementType) : base(elementType)
        {
            callbackOnRelease = false;
        }

        protected override void ReActiveElement()
        {
            FlowSubject.Event(elementType).RaiseOnRelease(true);
            baseElement.runtimeInstance.SetActive(false);
            baseElement.runtimeInstance.SetActive(true);
            callbackOnRelease = true;
            OnLoadResult(baseElement.runtimeInstance);
        }

        protected override void ActiveElement()
        {
            baseElement.runtimeInstance.SetActive(true);
            ElementsRuntimeManager.AddElement(baseElement);
            callbackOnRelease = true;
            OnLoadResult(baseElement.runtimeInstance);
        }

        internal override void OnRelease()
        {
            if (!callbackOnRelease) return;
            if (ReferenceEquals(sendData, null))
            {
                FlowSubject.Event(elementType).RaiseOnActive();
                return;
            }

            FlowSubject.Event(elementType).RaiseOnActiveWithData(sendData);
        }
    }
}
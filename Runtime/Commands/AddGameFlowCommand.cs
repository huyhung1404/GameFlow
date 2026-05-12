using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class AddGameFlowCommand : AddCommand
    {
        protected override GameFlowElement BaseElement { get; set; }

        public AddGameFlowCommand(Type elementType) : base(elementType)
        {
        }

        protected override void ReActiveElement()
        {
            BaseElement.EnsureCallbackEvent();
            BaseElement.CallbackEvent.RaiseOnRelease(true);
            BaseElement.RuntimeInstance.SetActive(false);
            BaseElement.RuntimeInstance.SetActive(true);
            _callbackOnRelease = true;
            Release();
        }

        protected override void ActiveElement()
        {
            BaseElement.EnsureCallbackEvent();
            BaseElement.RuntimeInstance.SetActive(true);
            Context.ElementsRuntime.AddElement(BaseElement);
            _callbackOnRelease = true;
            Release();
        }
    }

    internal sealed class AddGameFlowCommand<TData> : AddGameFlowCommand
    {
        private readonly TData _data;

        internal AddGameFlowCommand(Type elementType, TData data) : base(elementType)
        {
            _data = data;
        }

        internal override void RaiseActiveData(ElementCallbackEvent delegates) => delegates.RaiseOnActiveWithData(_data);
#if UNITY_EDITOR
        internal override string GetActiveDataInfo() => _data?.ToString() ?? "null";
#endif
    }
}

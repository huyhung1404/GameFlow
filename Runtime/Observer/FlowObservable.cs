using System;
using System.Collections.Generic;
using GameFlow.Internal;

namespace GameFlow
{
    public static class FlowObservable
    {
        private static Dictionary<Type, ElementCallbackEvent> CallbackEvents
            => GameFlowContext.Current?.CallbackEvents;

        public static ElementCallbackEvent Event<T>() where T : GameFlowElement
        {
            return Event(typeof(T));
        }

        public static ElementCallbackEvent Event(Type type)
        {
            var events = CallbackEvents;
            if (events == null)
            {
                ErrorHandle.LogError("FlowObservable: GameFlowContext is not initialized.");
                return type.IsSubclassOf(GameCommand.s_UIElementType) ? new UIElementCallbackEvent() : new ElementCallbackEvent();
            }

            if (events.TryGetValue(type, out var elementCallbackEvent))
            {
                return elementCallbackEvent;
            }

            var elementEvent = type.IsSubclassOf(GameCommand.s_UIElementType) ? new UIElementCallbackEvent() : new ElementCallbackEvent();
            events.Add(type, elementEvent);
            return elementEvent;
        }

        public static UIElementCallbackEvent UIEvent<T>() where T : UIFlowElement
        {
            return (UIElementCallbackEvent)Event(typeof(T));
        }

        internal static UIElementCallbackEvent UIEvent(Type type)
        {
            var e = Event(type);
            if (e is not UIElementCallbackEvent uiEvent)
                throw new InvalidOperationException($"FlowObservable.UIEvent: type '{type.Name}' is not a UIFlowElement.");
            return uiEvent;
        }

        public static void ReleaseEvent<T>() where T : GameFlowElement
        {
            ReleaseEvent(typeof(T));
        }

        public static bool ReleaseEvent(Type type)
        {
            return CallbackEvents?.Remove(type) ?? false;
        }
    }
}

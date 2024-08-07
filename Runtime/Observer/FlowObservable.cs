﻿using System;
using System.Collections.Generic;

namespace GameFlow
{
    public static class FlowObservable
    {
        internal static readonly Dictionary<Type, ElementCallbackEvent> callbackEvents = new Dictionary<Type, ElementCallbackEvent>();

        public static ElementCallbackEvent Event<T>() where T : GameFlowElement
        {
            return Event(typeof(T));
        }

        public static ElementCallbackEvent Event(Type type)
        {
            if (callbackEvents.TryGetValue(type, out var elementCallbackEvent))
            {
                return elementCallbackEvent;
            }

            var elementEvent = type.IsSubclassOf(GameCommand.UIElementType) ? new UIElementCallbackEvent() : new ElementCallbackEvent();
            callbackEvents.Add(type, elementEvent);
            return elementEvent;
        }

        public static UIElementCallbackEvent UIEvent<T>() where T : UIFlowElement
        {
            return (UIElementCallbackEvent)Event(typeof(T));
        }

        internal static UIElementCallbackEvent UIEvent(Type type)
        {
            return (UIElementCallbackEvent)Event(type);
        }

        public static void ReleaseEvent<T>() where T : GameFlowElement
        {
            ReleaseEvent(typeof(T));
        }

        public static bool ReleaseEvent(Type type)
        {
            return callbackEvents.Remove(type);
        }
    }
}
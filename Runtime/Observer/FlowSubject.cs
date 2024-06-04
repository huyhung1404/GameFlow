using System;
using System.Collections.Generic;

namespace GameFlow
{
    public static class FlowSubject
    {
        internal static List<ElementCallbackEvent> events = new List<ElementCallbackEvent>();

        public static ElementCallbackEvent Event<T>(string id = null) where T : GameFlowElement
        {
            return GetEventPool<T>(id);
        }

        public static UIElementCallbackEvent UIEvent<T>(string id = null) where T : UserInterfaceFlowElement
        {
            return (UIElementCallbackEvent)GetEventPool<T>(id);
        }

        public static ElementCallbackEvent Event(Type type, string id = null)
        {
            return GetEventPool(type, id);
        }

        public static void ReleaseEvent<T>(string id = null) where T : GameFlowElement
        {
            Remove(typeof(T), id);
        }

        private static ElementCallbackEvent GetEventPool<T>(string id) where T : GameFlowElement
        {
            var type = typeof(T);
            return TryGetValue(type, id, out var eventPool) ? eventPool : Add(type, id);
        }

        private static ElementCallbackEvent GetEventPool(Type type, string id)
        {
            return TryGetValue(type, id, out var eventPool) ? eventPool : Add(type, id);
        }

        private static bool TryGetValue(Type type, string id, out ElementCallbackEvent callbackEvent)
        {
            for (var i = events.Count - 1; i >= 0; i--)
            {
                if (type != events[i].type || !EventEquals(id, events[i].id)) continue;
                callbackEvent = events[i];
                return true;
            }

            callbackEvent = null;
            return false;
        }

        private static bool EventEquals(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2)) return true;
            return string.Equals(s1, s2);
        }

        private static ElementCallbackEvent Add(Type type, string id)
        {
            var elementEvent = type.IsSubclassOf(GameCommand.UIElementType)
                ? new UIElementCallbackEvent(type, string.IsNullOrEmpty(id) ? null : id)
                : new ElementCallbackEvent(type, string.IsNullOrEmpty(id) ? null : id);
            events.Add(elementEvent);
            return elementEvent;
        }

        private static void Remove(Type type, string id)
        {
            for (var i = events.Count - 1; i >= 0; i--)
            {
                if (type != events[i].type || !EventEquals(id, events[i].id)) continue;
                events.RemoveAt(i);
                return;
            }
        }
    }
}
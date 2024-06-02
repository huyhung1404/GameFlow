using System;

namespace GameFlow
{
    public static class GameFlowEvent
    {
        internal static EventCollection events = new EventCollection();

        public static void Listen<T>(OnActive onActive, string id = null) where T : GameFlowElement
        {
            GetEventPool<T>(id).onActive += onActive;
        }

        public static void Listen<T>(OnClose onClose, string id = null) where T : GameFlowElement
        {
            GetEventPool<T>(id).onClose += onClose;
        }

        public static void RemoveListener<T>(OnActive onActive, string id = null) where T : GameFlowElement
        {
            if (!events.TryGetValue(typeof(T), id, out var eventPool)) return;
            eventPool.onActive -= onActive;
        }

        public static void RemoveListener<T>(OnClose onClose, string id = null) where T : GameFlowElement
        {
            if (!events.TryGetValue(typeof(T), id, out var eventPool)) return;
            eventPool.onClose -= onClose;
        }

        internal static void OnActive(Type type, string id, object data)
        {
            if (!events.TryGetValue(type, id, out var eventPool)) return;
            eventPool.onActive?.Invoke(data);
        }

        internal static void OnClose(Type type, string id, bool closeIgnoreAnimation)
        {
            if (!events.TryGetValue(type, id, out var eventPool)) return;
            eventPool.onClose?.Invoke(closeIgnoreAnimation);
        }

        public static void Release<T>(string id = null) where T : GameFlowElement
        {
            events.Remove(typeof(T), id);
        }

        private static ElementEventPool GetEventPool<T>(string id) where T : GameFlowElement
        {
            var type = typeof(T);
            return events.TryGetValue(type, id, out var eventPool) ? eventPool : events.Add(type, id);
        }
    }
}
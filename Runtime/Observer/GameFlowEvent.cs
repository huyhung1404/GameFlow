using System;
using System.Collections.Generic;

namespace GameFlow
{
    public static class GameFlowEvent
    {
        private static readonly Dictionary<Type, ElementEventPool> events = new Dictionary<Type, ElementEventPool>();

        public static void Listen<T>(OnActive onActive, string id = null) where T : GameFlowElement
        {
            GetEventPool<T>().onActive += onActive;
        }

        public static void Listen<T>(OnClose onClose, string id = null) where T : GameFlowElement
        {
            GetEventPool<T>().onClose += onClose;
        }

        public static void RemoveListener<T>(OnActive onActive, string id = null) where T : GameFlowElement
        {
            if (!events.TryGetValue(typeof(T), out var eventPool)) return;
            eventPool.onActive -= onActive;
        }

        public static void RemoveListener<T>(OnClose onClose, string id = null) where T : GameFlowElement
        {
            if (!events.TryGetValue(typeof(T), out var eventPool)) return;
            eventPool.onClose -= onClose;
        }

        internal static void OnActive(Type type, string id, object data)
        {
            if (!events.TryGetValue(type, out var eventPool)) return;
            eventPool.onActive?.Invoke(data);
        }

        internal static void OnClose(Type type, string id, bool closeIgnoreAnimation)
        {
            if (!events.TryGetValue(type, out var eventPool)) return;
            eventPool.onClose?.Invoke(closeIgnoreAnimation);
        }

        public static void Release<T>() where T : GameFlowElement
        {
            events.Remove(typeof(T));
        }

        private static ElementEventPool GetEventPool<T>() where T : GameFlowElement
        {
            var type = typeof(T);
            if (events.TryGetValue(type, out var eventPool))
            {
                return eventPool;
            }

            eventPool = new ElementEventPool();
            events.Add(type, eventPool);
            return eventPool;
        }
    }
}
using System;
using System.Collections.Generic;

namespace GameFlow
{
    public static class GameFlowEvent
    {
        private static readonly Dictionary<Type, ElementEventPool> events = new Dictionary<Type, ElementEventPool>();

        public static void Listen<T>(OnActive onActive) where T : GameFlowElement
        {
            GetEventPool<T>().onActive += onActive;
        }

        public static void RemoveListener<T>(OnActive onActive) where T : GameFlowElement
        {
            if (!events.TryGetValue(typeof(T), out var eventPool)) return;
            eventPool.onActive -= onActive;
        }

        internal static void OnActive(Type type, object data)
        {
            if (!events.TryGetValue(type, out var eventPool)) return;
            eventPool.onActive?.Invoke(data);
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
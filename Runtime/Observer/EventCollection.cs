using System;
using System.Collections.Generic;

namespace GameFlow
{
    internal class EventCollection
    {
        private readonly List<ElementEventPool> pool = new List<ElementEventPool>();

        internal bool TryGetValue(Type type, string id, out ElementEventPool eventPool)
        {
            for (var i = pool.Count - 1; i >= 0; i--)
            {
                if (type != pool[i].type || !EventEquals(id, pool[i].id)) continue;
                eventPool = pool[i];
                return true;
            }

            eventPool = null;
            return false;
        }

        private static bool EventEquals(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2)) return true;
            return string.Equals(s1, s2);
        }

        internal ElementEventPool Add(Type type, string id)
        {
            var elementEvent = new ElementEventPool(type, string.IsNullOrEmpty(id) ? null : id);
            pool.Add(elementEvent);
            return elementEvent;
        }

        internal void Remove(Type type, string id)
        {
            for (var i = pool.Count - 1; i >= 0; i--)
            {
                if (type != pool[i].type || !EventEquals(id, pool[i].id)) continue;
                pool.RemoveAt(i);
                return;
            }
        }
    }
}
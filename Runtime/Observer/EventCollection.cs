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
                if (type != pool[i].type || !string.Equals(id, pool[i].id)) continue;
                eventPool = pool[i];
                return true;
            }

            eventPool = null;
            return false;
        }

        internal ElementEventPool Add(Type type, string id)
        {
            var elementEvent = new ElementEventPool(type, id);
            pool.Add(elementEvent);
            return elementEvent;
        }

        internal void Remove(Type type, string id)
        {
            for (var i = pool.Count - 1; i >= 0; i--)
            {
                if (type != pool[i].type || !string.Equals(id, pool[i].id)) continue;
                pool.RemoveAt(i);
                return;
            }
        }
    }
}
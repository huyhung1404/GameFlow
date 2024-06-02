using System;

namespace GameFlow
{
    internal class ElementEventPool
    {
        internal readonly Type type;
        internal readonly string id;
        internal OnActive onActive;
        internal OnClose onClose;

        public ElementEventPool(Type type, string id)
        {
            this.type = type;
            if (string.IsNullOrEmpty(id)) id = null;
            this.id = id;
        }
    }
}
using System;

namespace GameFlow
{
    public sealed class ElementCallbackEvent
    {
        internal readonly Type type;
        internal readonly string id;

        internal ElementCallbackEvent(Type type, string id)
        {
            this.type = type;
            this.id = string.IsNullOrEmpty(id) ? null : id;
        }

        internal Action onActive;
        internal Action<object> onActiveWithData;
        internal Action<bool> onRelease;
        public event Action OnActive { add => onActive += value; remove => onActive -= value; }
        public event Action<object> OnActiveWithData { add => onActiveWithData += value; remove => onActiveWithData -= value; }

        /// <summary>
        /// Return true if release immediately
        /// </summary>
        public event Action<bool> OnRelease { add => onRelease += value; remove => onRelease -= value; }
    }
}
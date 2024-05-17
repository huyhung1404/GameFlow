using System;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    public class ElementCollection : ISerializationCallbackReceiver
    {
        [SerializeField] private GameFlowElement[] elements;
        private Type[] types;
        private int elementCount;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (elements == null) return;
            elementCount = elements.Length;
            types = new Type[elementCount];
            for (var i = 0; i < elementCount; i++)
            {
                types[i] = elements[i].GetType();
            }
        }

        internal GameFlowElement GetElement(Type type)
        {
            for (var i = 0; i < elementCount; i++)
            {
                if (type != types[i]) continue;
                var element = elements[i];
#if !UNITY_EDITOR
                if (!element.includeInBuild) return null;
#endif
                return element;
            }

            return null;
        }
    }
}
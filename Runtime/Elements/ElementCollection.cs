using System;
using System.Linq;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    internal class ElementCollection : ISerializationCallbackReceiver
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

        internal void GenerateElement(GameFlowElement element)
        {
            var listElements = elements.ToList();
            var isAdd = true;
            for (var i = 0; i < listElements.Count - 1; i++)
            {
                if (listElements[i].GetType() != element.GetType()) continue;
                listElements.Insert(i + 1, element);
                isAdd = false;
                break;
            }

            if (isAdd) listElements.Add(element);
            elements = listElements.ToArray();
            OnAfterDeserialize();
        }
    }
}
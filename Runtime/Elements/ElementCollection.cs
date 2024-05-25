using System;
using System.Linq;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    internal class ElementCollection : ISerializationCallbackReceiver
    {
        [SerializeReference] private GameFlowElement[] elements;
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
                types[i] = elements[i]?.GetType();
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

        internal GameFlowElement GetElement(Type type, string id)
        {
            for (var i = 0; i < elementCount; i++)
            {
                if (type != types[i]) continue;
                var element = elements[i];
                if (!string.Equals(element.instanceID, id)) continue;
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

        internal bool VerifyData()
        {
            if (elements == null) return false;
            var isChange = false;
            var listElements = elements.ToList();
            for (var i = listElements.Count - 1; i >= 0; i--)
            {
                if (listElements[i] != null) continue;
                listElements.RemoveAt(i);
                isChange = true;
            }

            elements = listElements.ToArray();
            OnAfterDeserialize();
            return isChange;
        }
    }
}
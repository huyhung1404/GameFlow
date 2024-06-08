using System;
using System.Linq;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    internal class ElementCollection
    {
        [SerializeField] private GameFlowElement[] elements;

        internal GameFlowElement GetIndex(int index)
        {
            return index >= elements.Length ? null : elements[index];
        }

        internal GameFlowElement GetElement(Type type)
        {
            var elementCount = elements.Length;
            for (var i = 0; i < elementCount; i++)
            {
                var element = elements[i];
                if (type != element.elementType) continue;
#if !UNITY_EDITOR
                if (!element.includeInBuild) return null;
#endif
                return element;
            }

            return null;
        }

        internal bool TryGetElement(Type type, out GameFlowElement element)
        {
            element = GetElement(type);
            return !ReferenceEquals(element, null);
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
            return isChange;
        }
    }
}
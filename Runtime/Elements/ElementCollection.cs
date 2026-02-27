using System;
using System.Linq;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    internal class ElementCollection
    {
        [SerializeField] private GameFlowElement[] m_elements;

        internal GameFlowElement GetIndex(int index)
        {
            return index >= m_elements.Length ? null : m_elements[index];
        }

        internal GameFlowElement GetElement(Type type)
        {
            var elementCount = m_elements.Length;
            for (var i = 0; i < elementCount; i++)
            {
                var element = m_elements[i];
                if (type != element.ElementType) continue;
#if !UNITY_EDITOR
                if (!element.IncludeInBuild) return null;
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
            m_elements ??= Array.Empty<GameFlowElement>();
            var listElements = m_elements.ToList();
            var isAdd = true;
            for (var i = 0; i < listElements.Count - 1; i++)
            {
                if (listElements[i].GetType() != element.GetType()) continue;
                listElements.Insert(i + 1, element);
                isAdd = false;
                break;
            }

            if (isAdd) listElements.Add(element);
            m_elements = listElements.ToArray();
        }

        internal bool VerifyData()
        {
            if (m_elements == null) return false;
            var isChange = false;
            var listElements = m_elements.ToList();
            for (var i = listElements.Count - 1; i >= 0; i--)
            {
                if (listElements[i] != null) continue;
                listElements.RemoveAt(i);
                isChange = true;
            }

            m_elements = listElements.ToArray();
            return isChange;
        }
    }
}
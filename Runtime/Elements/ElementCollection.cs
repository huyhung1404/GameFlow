using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    internal class ElementCollection
    {
        private const int k_lookupCacheThreshold = 150;
        [SerializeField] private GameFlowElement[] m_elements;
        private Dictionary<Type, GameFlowElement> _lookupCache;

        private void InvalidateLookupCache()
        {
            _lookupCache = null;
        }

        private void EnsureLookupCache()
        {
            if (_lookupCache != null) return;
            if (m_elements == null || m_elements.Length <= k_lookupCacheThreshold) return;

            _lookupCache = new Dictionary<Type, GameFlowElement>();
            foreach (var element in m_elements)
            {
                if (ReferenceEquals(element, null)) continue;
                var t = element.ElementType;
                if (_lookupCache.ContainsKey(t)) continue;
                _lookupCache[t] = element;
            }
        }

        internal GameFlowElement GetIndex(int index)
        {
            if (index < 0 || index >= m_elements.Length) return null;
            return m_elements[index];
        }

        internal GameFlowElement GetElement(Type type)
        {
            if (m_elements.Length > k_lookupCacheThreshold)
            {
                EnsureLookupCache();
                if (_lookupCache != null && _lookupCache.TryGetValue(type, out var cached))
                {
#if !UNITY_EDITOR
                    if (!cached.IncludeInBuild) return null;
#endif
                    return cached;
                }

                return null;
            }

            var elementCount = m_elements.Length;
            for (var i = 0; i < elementCount; i++)
            {
                var element = m_elements[i];
                if (ReferenceEquals(element, null) || type != element.ElementType) continue;
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

#if UNITY_EDITOR

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
            InvalidateLookupCache();
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
            InvalidateLookupCache();
            return isChange;
        }
#endif
    }
}

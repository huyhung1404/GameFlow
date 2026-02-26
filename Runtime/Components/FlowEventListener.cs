using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameFlow.Component
{
    [AddComponentMenu("Game Flow/Event Listener")]
    public class FlowEventListener : ElementMonoBehaviours
    {
        internal enum EventTriggerType
        {
            OnActive,
            OnActiveWithData,
            OnShowCompleted,
            OnKeyBack,
            OnReFocus,
            OnRelease
        }

        [Serializable]
        internal abstract class Entry
        {
            [SerializeField] internal EventTriggerType eventID;
            internal abstract void Register(Type type);
            internal abstract void Unregister(Type type);
        }

        [Serializable]
        internal class OnActiveEntry : Entry
        {
            [SerializeField] internal UnityEvent callback;

            internal override void Register(Type type)
            {
                FlowObservable.Event(type).OnActive += callback.Invoke;
            }

            internal override void Unregister(Type type)
            {
                FlowObservable.Event(type).OnActive -= callback.Invoke;
            }
        }

        [Serializable]
        internal class OnActiveWithDataEntry : Entry
        {
            [SerializeField] internal UnityEvent<object> callback;

            internal override void Register(Type type)
            {
                FlowObservable.Event(type).OnActiveWithData += callback.Invoke;
            }

            internal override void Unregister(Type type)
            {
                FlowObservable.Event(type).OnActiveWithData -= callback.Invoke;
            }
        }

        [Serializable]
        internal class OnShowCompletedEntry : Entry
        {
            [SerializeField] internal UnityEvent callback;

            internal override void Register(Type type)
            {
                FlowObservable.UIEvent(type).OnShowCompleted += callback.Invoke;
            }

            internal override void Unregister(Type type)
            {
                FlowObservable.UIEvent(type).OnShowCompleted -= callback.Invoke;
            }
        }

        [Serializable]
        internal class OnKeyBackEntry : Entry
        {
            [SerializeField] internal UnityEvent callback;

            internal override void Register(Type type)
            {
                FlowObservable.UIEvent(type).OnKeyBack += callback.Invoke;
            }

            internal override void Unregister(Type type)
            {
                FlowObservable.UIEvent(type).OnKeyBack -= callback.Invoke;
            }
        }

        [Serializable]
        internal class OnReFocusEntry : Entry
        {
            [SerializeField] internal UnityEvent callback;

            internal override void Register(Type type)
            {
                FlowObservable.UIEvent(type).OnReFocus += callback.Invoke;
            }

            internal override void Unregister(Type type)
            {
                FlowObservable.UIEvent(type).OnReFocus -= callback.Invoke;
            }
        }

        [Serializable]
        internal class OnReleaseEntry : Entry
        {
            [SerializeField] internal UnityEvent<bool> callback;

            internal override void Register(Type type)
            {
                FlowObservable.Event(type).OnRelease += callback.Invoke;
            }

            internal override void Unregister(Type type)
            {
                FlowObservable.Event(type).OnRelease -= callback.Invoke;
            }
        }

        [SerializeField] private GameFlowElement m_element;
        [SerializeReference] private List<Entry> m_delegates;

        private void OnEnable()
        {
            var type = m_element.elementType;
            for (var i = m_delegates.Count - 1; i >= 0; i--) m_delegates[i].Register(type);
        }

        private void OnDisable()
        {
            var type = m_element.elementType;
            for (var i = m_delegates.Count - 1; i >= 0; i--) m_delegates[i].Unregister(type);
        }

        internal override void SetElement(GameFlowElement value, Type type)
        {
            if (m_element != null && type != m_element.GetType()) return;
            m_element = value;
        }
    }
}
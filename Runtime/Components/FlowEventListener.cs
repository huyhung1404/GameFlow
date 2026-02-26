using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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
            [SerializeField, FormerlySerializedAs("eventID")] internal EventTriggerType EventID;
            internal abstract void Register(Type type);
            internal abstract void Unregister(Type type);
        }

        [Serializable]
        internal class OnActiveEntry : Entry
        {
            [SerializeField, FormerlySerializedAs("callback")] internal UnityEvent Callback;

            internal override void Register(Type type)
            {
                FlowObservable.Event(type).OnActive += Callback.Invoke;
            }

            internal override void Unregister(Type type)
            {
                FlowObservable.Event(type).OnActive -= Callback.Invoke;
            }
        }

        [Serializable]
        internal class OnActiveWithDataEntry : Entry
        {
            [SerializeField, FormerlySerializedAs("callback")] internal UnityEvent<object> Callback;

            internal override void Register(Type type)
            {
                FlowObservable.Event(type).OnActiveWithData += Callback.Invoke;
            }

            internal override void Unregister(Type type)
            {
                FlowObservable.Event(type).OnActiveWithData -= Callback.Invoke;
            }
        }

        [Serializable]
        internal class OnShowCompletedEntry : Entry
        {
            [SerializeField, FormerlySerializedAs("callback")] internal UnityEvent Callback;

            internal override void Register(Type type)
            {
                FlowObservable.UIEvent(type).OnShowCompleted += Callback.Invoke;
            }

            internal override void Unregister(Type type)
            {
                FlowObservable.UIEvent(type).OnShowCompleted -= Callback.Invoke;
            }
        }

        [Serializable]
        internal class OnKeyBackEntry : Entry
        {
            [SerializeField, FormerlySerializedAs("callback")] internal UnityEvent Callback;

            internal override void Register(Type type)
            {
                FlowObservable.UIEvent(type).OnKeyBack += Callback.Invoke;
            }

            internal override void Unregister(Type type)
            {
                FlowObservable.UIEvent(type).OnKeyBack -= Callback.Invoke;
            }
        }

        [Serializable]
        internal class OnReFocusEntry : Entry
        {
            [SerializeField, FormerlySerializedAs("callback")] internal UnityEvent Callback;

            internal override void Register(Type type)
            {
                FlowObservable.UIEvent(type).OnReFocus += Callback.Invoke;
            }

            internal override void Unregister(Type type)
            {
                FlowObservable.UIEvent(type).OnReFocus -= Callback.Invoke;
            }
        }

        [Serializable]
        internal class OnReleaseEntry : Entry
        {
            [SerializeField, FormerlySerializedAs("callback")] internal UnityEvent<bool> Callback;

            internal override void Register(Type type)
            {
                FlowObservable.Event(type).OnRelease += Callback.Invoke;
            }

            internal override void Unregister(Type type)
            {
                FlowObservable.Event(type).OnRelease -= Callback.Invoke;
            }
        }

        [SerializeField] private GameFlowElement m_element;
        [SerializeReference] private List<Entry> m_delegates;

        private void OnEnable()
        {
            var type = m_element.ElementType;
            for (var i = m_delegates.Count - 1; i >= 0; i--) m_delegates[i].Register(type);
        }

        private void OnDisable()
        {
            var type = m_element.ElementType;
            for (var i = m_delegates.Count - 1; i >= 0; i--) m_delegates[i].Unregister(type);
        }

        internal override void SetElement(GameFlowElement value, Type type)
        {
            if (m_element != null && type != m_element.GetType()) return;
            m_element = value;
        }
    }
}
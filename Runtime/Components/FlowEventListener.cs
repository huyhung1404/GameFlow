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

        [SerializeField] private GameFlowElement element;
        [SerializeReference] internal List<Entry> delegates;

        private void OnEnable()
        {
            var type = element.elementType;
            for (var i = delegates.Count - 1; i >= 0; i--) delegates[i].Register(type);
        }

        private void OnDisable()
        {
            var type = element.elementType;
            for (var i = delegates.Count - 1; i >= 0; i--) delegates[i].Unregister(type);
        }

        internal override void SetElement(GameFlowElement value, Type type)
        {
            if (element != null && type != element.GetType()) return;
            element = value;
        }
    }
}
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
            [SerializeField] internal EventTriggerType EventID;
            internal abstract void Register(ElementCallbackEvent callbackEvent);
            internal abstract void Unregister(ElementCallbackEvent callbackEvent);
        }

        [Serializable]
        internal class OnActiveEntry : Entry
        {
            [SerializeField] internal UnityEvent Callback;

            internal override void Register(ElementCallbackEvent callbackEvent)
            {
                callbackEvent.OnActive += Callback.Invoke;
            }

            internal override void Unregister(ElementCallbackEvent callbackEvent)
            {
                callbackEvent.OnActive -= Callback.Invoke;
            }
        }

        [Serializable]
        internal class OnActiveWithDataEntry : Entry
        {
            [SerializeField] internal UnityEvent<object> Callback;

            internal override void Register(ElementCallbackEvent callbackEvent)
            {
                callbackEvent.OnActiveWithData += Callback.Invoke;
            }

            internal override void Unregister(ElementCallbackEvent callbackEvent)
            {
                callbackEvent.OnActiveWithData -= Callback.Invoke;
            }
        }

        [Serializable]
        internal class OnShowCompletedEntry : Entry
        {
            [SerializeField] internal UnityEvent Callback;

            internal override void Register(ElementCallbackEvent callbackEvent)
            {
                if (callbackEvent is UIElementCallbackEvent uiEvent)
                    uiEvent.OnShowCompleted += Callback.Invoke;
            }

            internal override void Unregister(ElementCallbackEvent callbackEvent)
            {
                if (callbackEvent is UIElementCallbackEvent uiEvent)
                    uiEvent.OnShowCompleted -= Callback.Invoke;
            }
        }

        [Serializable]
        internal class OnKeyBackEntry : Entry
        {
            [SerializeField] internal UnityEvent Callback;

            internal override void Register(ElementCallbackEvent callbackEvent)
            {
                if (callbackEvent is UIElementCallbackEvent uiEvent)
                    uiEvent.OnKeyBack += Callback.Invoke;
            }

            internal override void Unregister(ElementCallbackEvent callbackEvent)
            {
                if (callbackEvent is UIElementCallbackEvent uiEvent)
                    uiEvent.OnKeyBack -= Callback.Invoke;
            }
        }

        [Serializable]
        internal class OnReFocusEntry : Entry
        {
            [SerializeField] internal UnityEvent Callback;

            internal override void Register(ElementCallbackEvent callbackEvent)
            {
                if (callbackEvent is UIElementCallbackEvent uiEvent)
                    uiEvent.OnReFocus += Callback.Invoke;
            }

            internal override void Unregister(ElementCallbackEvent callbackEvent)
            {
                if (callbackEvent is UIElementCallbackEvent uiEvent)
                    uiEvent.OnReFocus -= Callback.Invoke;
            }
        }

        [Serializable]
        internal class OnReleaseEntry : Entry
        {
            [SerializeField] internal UnityEvent<bool> Callback;

            internal override void Register(ElementCallbackEvent callbackEvent)
            {
                callbackEvent.OnRelease += Callback.Invoke;
            }

            internal override void Unregister(ElementCallbackEvent callbackEvent)
            {
                callbackEvent.OnRelease -= Callback.Invoke;
            }
        }

        [SerializeField] private GameFlowElement m_element;
        [SerializeReference] private List<Entry> m_delegates;
        private ElementCallbackEvent _callbackEvent;

        private void OnEnable()
        {
            m_element.EnsureCallbackEvent();
            _callbackEvent = m_element.CallbackEvent;
            for (var i = m_delegates.Count - 1; i >= 0; i--) m_delegates[i].Register(_callbackEvent);
        }

        private void OnDisable()
        {
            if (_callbackEvent == null) return;
            for (var i = m_delegates.Count - 1; i >= 0; i--) m_delegates[i].Unregister(_callbackEvent);
        }

        internal override void SetElement(GameFlowElement value, Type type)
        {
            if (m_element != null && type != m_element.GetType()) return;

            var wasRegistered = _callbackEvent != null && isActiveAndEnabled;
            if (wasRegistered)
            {
                for (var i = m_delegates.Count - 1; i >= 0; i--) m_delegates[i].Unregister(_callbackEvent);
            }

            m_element = value;
            m_element.EnsureCallbackEvent();
            _callbackEvent = m_element.CallbackEvent;

            if (wasRegistered)
            {
                for (var i = m_delegates.Count - 1; i >= 0; i--) m_delegates[i].Register(_callbackEvent);
            }
        }
    }
}

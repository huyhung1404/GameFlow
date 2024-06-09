using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFlow.Component
{
    public class FlowEventListener : MonoBehaviour
    {
        public enum EventTriggerType
        {
        }

        [Serializable]
        public abstract class Entry
        {
            public EventTriggerType eventId;
            public abstract void Register(Type type);
            public abstract void Unregister(Type type);
        }

        [SerializeField] private GameFlowElement element;
        [SerializeReference] private List<Entry> delegates;

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
    }
}
using UnityEngine;

namespace GameFlow.Tests
{
    public class FlowCallbackMonoBehaviour : MonoBehaviour
    {
        public GameFlowElement element;

        private void Awake()
        {
            CallbackHistory.current.RecorderObject(gameObject, element);
        }

        private void OnEnable()
        {
            var delegates = FlowSubject.Event(element.GetType());
            delegates.OnActive += OnActive;
            delegates.OnActiveWithData += OnActiveWithData;
            delegates.OnRelease += OnRelease;
        }

        private void OnActive()
        {
            CallbackHistory.current.WriteOnActive(element.GetType());
        }

        private void OnActiveWithData(object obj)
        {
            CallbackHistory.current.WriteOnActiveWithData(element.GetType(), obj);
        }

        private void OnRelease(bool obj)
        {
            CallbackHistory.current.WriteOnRelease(element.GetType(), obj);
        }

        private void OnDisable()
        {
            var delegates = FlowSubject.Event(element.GetType());
            delegates.OnActive -= OnActive;
            delegates.OnActiveWithData -= OnActiveWithData;
            delegates.OnRelease -= OnRelease;
        }
    }
}
using UnityEngine;

namespace GameFlow.Tests
{
    public class FlowCallbackMonoBehaviour : MonoBehaviour
    {
        public GameFlowElement element;

        private void Awake()
        {
            CallbackHistory.Current.RecorderObject(gameObject, element);
        }

        private void OnEnable()
        {
            var delegates = FlowObservable.Event(element.GetType());
            delegates.OnActive += OnActive;
            delegates.OnActiveWithData += OnActiveWithData;
            delegates.OnRelease += OnRelease;
        }

        private void OnActive()
        {
            CallbackHistory.Current.WriteOnActive(element.GetType());
        }

        private void OnActiveWithData(object obj)
        {
            CallbackHistory.Current.WriteOnActiveWithData(element.GetType(), obj);
        }

        private void OnRelease(bool obj)
        {
            CallbackHistory.Current.WriteOnRelease(element.GetType(), obj);
        }

        private void OnDisable()
        {
            var delegates = FlowObservable.Event(element.GetType());
            delegates.OnActive -= OnActive;
            delegates.OnActiveWithData -= OnActiveWithData;
            delegates.OnRelease -= OnRelease;
        }
    }
}
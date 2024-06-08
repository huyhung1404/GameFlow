using UnityEngine;

namespace GameFlow.Tests
{
    [AddComponentMenu("")]
    public class TestMonoBehaviour<T> : MonoBehaviour where T : GameFlowElement
    {
        public string id;
        public int onActiveCount;
        public int onCloseCount;
        public bool onEnable;

        private void OnEnable()
        {
            onEnable = true;
            FlowSubject.Event<T>().OnActive += OnActive;
            FlowSubject.Event<T>().OnRelease += OnClose;
        }

        private void OnActive()
        {
            onActiveCount++;
        }

        private void OnClose(bool ignoreAnimation)
        {
            onCloseCount++;
        }

        private void OnDisable()
        {
            onEnable = false;
            FlowSubject.Event<T>().OnActive -= OnActive;
            FlowSubject.Event<T>().OnRelease -= OnClose;
        }
    }
}
using UnityEngine;

namespace GameFlow.Tests
{
    [AddComponentMenu("")]
    public class PrefabTestMonoBehaviour : MonoBehaviour
    {
        public string id;
        public int onActiveCount;
        public int onCloseCount;
        public bool onEnable;

        public static PrefabTestMonoBehaviour GetWithID(string idSearch)
        {
            foreach (var mono in FindObjectsOfType<PrefabTestMonoBehaviour>())
            {
                if (string.IsNullOrEmpty(mono.id) && string.IsNullOrEmpty(idSearch)) return mono;
                if (mono.id == idSearch) return mono;
            }

            return null;
        }

        private void OnEnable()
        {
            onEnable = true;
            FlowSubject.Event<AddGameFlowCommand_Tests.TestScript___ElementAddPrefab>(id).OnActive += OnActive;
            FlowSubject.Event<AddGameFlowCommand_Tests.TestScript___ElementAddPrefab>(id).OnRelease += OnClose;
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
            FlowSubject.Event<AddGameFlowCommand_Tests.TestScript___ElementAddPrefab>(id).OnActive -= OnActive;
            FlowSubject.Event<AddGameFlowCommand_Tests.TestScript___ElementAddPrefab>(id).OnRelease -= OnClose;
        }
    }
}
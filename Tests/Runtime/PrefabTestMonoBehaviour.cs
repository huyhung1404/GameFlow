using UnityEngine;

namespace GameFlow.Tests
{
    [AddComponentMenu("")]
    public class PrefabTestMonoBehaviour : MonoBehaviour
    {
        public static int onActiveCount;
        public static int onCloseCount;
        public static bool onEnable;

        private void OnEnable()
        {
            onEnable = true;
            FlowSubject.Active.Listen<AddGameFlowCommand_Tests.TestScript___ElementAddPrefab>(OnActive);
            FlowSubject.Close.Listen<AddGameFlowCommand_Tests.TestScript___ElementAddPrefab>(OnClose);
        }

        private static void OnActive(object data)
        {
            onActiveCount++;
        }

        private static void OnClose(bool ignoreAnimation)
        {
            onCloseCount++;
        }

        private void OnDisable()
        {
            onEnable = false;
            FlowSubject.Active.RemoveListener<AddGameFlowCommand_Tests.TestScript___ElementAddPrefab>(OnActive);
            FlowSubject.Close.RemoveListener<AddGameFlowCommand_Tests.TestScript___ElementAddPrefab>(OnClose);
        }
    }
}
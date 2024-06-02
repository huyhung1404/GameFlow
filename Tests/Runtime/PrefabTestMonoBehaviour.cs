using System;
using UnityEngine;

namespace GameFlow.Tests
{
    [AddComponentMenu("")]
    public class PrefabTestMonoBehaviour : MonoBehaviour
    {
        public static int onActiveCount = 0;
        public static int onCloseCount = 0;

        private void OnEnable()
        {
            GameFlowEvent.Listen<AddGameFlowCommand_Tests.TestScript___ElementAddPrefab>(onActive: data =>
            {
                onActiveCount++;
            });
        }

        private void OnDisable()
        {
            GameFlowEvent.RemoveListener<AddGameFlowCommand_Tests.TestScript___ElementAddPrefab>(onClose: forceRelease =>
            {
                onCloseCount++;
            });
        }
    }
}
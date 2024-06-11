using UnityEngine;

namespace GameFlow.Tests
{
    [AddComponentMenu("")]
    public class PrefabTestMonoBehaviour : TestMonoBehaviour<AddGameFlowCommand_Tests.TestScript___ElementAddPrefab>
    {
        public static PrefabTestMonoBehaviour GetWithID(string idSearch)
        {
            foreach (var mono in FindObjectsOfType<PrefabTestMonoBehaviour>())
            {
                if (string.IsNullOrEmpty(mono.id) && string.IsNullOrEmpty(idSearch)) return mono;
                if (mono.id == idSearch) return mono;
            }

            return null;
        }
    }
}
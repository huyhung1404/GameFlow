using UnityEngine;

namespace GameFlow.Internal
{
    [AddComponentMenu("")]
    internal class SceneElementHandle : MonoBehaviour
    {
        internal static SceneElementHandle Create(ElementReleaseMode releaseMode)
        {
            var handle = new GameObject().AddComponent<SceneElementHandle>();
#if UNITY_EDITOR
            handle.name = "Scene Handle";
#endif
            handle.releaseMode = releaseMode;
            return handle;
        }

        private struct ActiveData
        {
            public GameObject o;
            public bool isActive;
        }

        private ActiveData[] activeData;
        private ElementReleaseMode releaseMode;

        internal void GetRootsGameObjectIfNeed()
        {
            if (activeData != null) return;
            var root = gameObject.scene.GetRootGameObjects();
            var rootLength = root.Length;
            activeData = new ActiveData[rootLength - 1];
            var index = 0;
            for (var i = 0; i < rootLength; i++)
            {
                var o = root[i];
                if (gameObject == o) continue;
                activeData[index] = new ActiveData
                {
                    o = o,
                    isActive = o.activeSelf
                };
                index++;
            }
        }

        private void OnEnable()
        {
            if (activeData == null) return;
            for (var i = activeData.Length - 1; i >= 0; i--)
            {
                activeData[i].o.SetActive(activeData[i].isActive);
            }
        }

        private void OnDisable()
        {
            if (releaseMode == ElementReleaseMode.NONE_RELEASE) GetRootsGameObjectIfNeed();
            if (activeData == null) return;
            for (var i = activeData.Length - 1; i >= 0; i--)
            {
                if (activeData[i].o) activeData[i].o.SetActive(false);
            }
        }
    }
}
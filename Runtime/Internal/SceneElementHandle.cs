using UnityEngine;

namespace GameFlow.Internal
{
    [AddComponentMenu("")]
    internal class SceneElementHandle : MonoBehaviour
    {
        internal static GameObject Create()
        {
            var handle = Instantiate(new GameObject()).AddComponent<SceneElementHandle>();
#if UNITY_EDITOR
            handle.name = "Scene Handle";
#endif
            return handle.gameObject;
        }

        private struct ActiveData
        {
            public GameObject o;
            public bool isActive;
        }

        private ActiveData[] activeData;
        private bool isActive;

        private void Awake()
        {
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

            isActive = true;
        }

        private void OnEnable()
        {
            if (isActive)
            {
                isActive = false;
                return;
            }

            for (var i = activeData.Length - 1; i >= 0; i--)
            {
                activeData[i].o.SetActive(activeData[i].isActive);
            }
        }

        private void OnDisable()
        {
            for (var i = activeData.Length - 1; i >= 0; i--)
            {
                if (activeData[i].o) activeData[i].o.SetActive(false);
            }
        }
    }
}
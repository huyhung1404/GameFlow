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
            handle._releaseMode = releaseMode;
            return handle;
        }

        private struct ActiveData
        {
            public GameObject O;
            public bool IsActive;
        }

        private ActiveData[] _activeData;
        private ElementReleaseMode _releaseMode;

        internal void GetRootsGameObjectIfNeed()
        {
            if (_activeData != null) return;
            var root = gameObject.scene.GetRootGameObjects();
            var rootLength = root.Length;
            _activeData = new ActiveData[rootLength - 1];
            var index = 0;
            for (var i = 0; i < rootLength; i++)
            {
                var o = root[i];
                if (gameObject == o) continue;
                _activeData[index] = new ActiveData
                {
                    O = o,
                    IsActive = o.activeSelf
                };
                index++;
            }
        }

        private void OnEnable()
        {
            if (_activeData == null) return;
            for (var i = _activeData.Length - 1; i >= 0; i--)
            {
                _activeData[i].O.SetActive(_activeData[i].IsActive);
            }
        }

        private void OnDisable()
        {
            if (_releaseMode == ElementReleaseMode.NoneRelease) GetRootsGameObjectIfNeed();
            if (_activeData == null) return;
            for (var i = _activeData.Length - 1; i >= 0; i--)
            {
                if (_activeData[i].O) _activeData[i].O.SetActive(false);
            }
        }
    }
}
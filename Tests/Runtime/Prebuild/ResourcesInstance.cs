using System.Collections;
using GameFlow.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFlow.Tests.Build
{
    internal static class ResourcesInstance
    {
        internal static Scene managerScene { get; private set; }
        internal static GameObject root { get; private set; }
        internal static GameFlowManager manager { get; private set; }
        internal static GameFlowRuntimeController runtimeController { get; private set; }
        internal static LoadingController loadingController { get; private set; }
        internal static DisplayLoading imageLoading { get; private set; }
        internal static FadeLoading fadeLoading { get; private set; }
        internal static ProgressLoading progressLoading { get; private set; }

        public static IEnumerator Load()
        {
            SceneManager.LoadScene("TestManager", LoadSceneMode.Additive);
            managerScene = SceneManager.GetSceneByName("TestManager");
            yield return null;
            foreach (var o in DontDestroyOnLoadObjects())
            {
                runtimeController = o.GetComponent<GameFlowRuntimeController>();
                if (runtimeController != null) break;
            }

            root = runtimeController.gameObject;
            loadingController = root.GetComponentInChildren<LoadingController>();
            imageLoading = runtimeController.GetComponentInChildren<DisplayLoading>();
            fadeLoading = runtimeController.GetComponentInChildren<FadeLoading>();
            progressLoading = runtimeController.GetComponentInChildren<ProgressLoading>();
            const float timeOut = 10;
            var currentTimeOut = 0f;
            while (!runtimeController.isActive || currentTimeOut >= timeOut)
            {
                yield return null;
                currentTimeOut += Time.deltaTime;
            }

            if (currentTimeOut >= timeOut) Debug.LogError("Time out!");
            manager = GameFlowRuntimeController.Manager();
        }

        public static GameObject[] DontDestroyOnLoadObjects()
        {
            var go = new GameObject("Sacrificial Lamb");
            Object.DontDestroyOnLoad(go);
            return go.scene.GetRootGameObjects();
        }
    }
}
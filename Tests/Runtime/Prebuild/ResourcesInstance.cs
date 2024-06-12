using System.Collections;
using System.Collections.Generic;
using GameFlow.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

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
            SceneManager.LoadScene(Prebuild.kSceneBuildPath, LoadSceneMode.Additive);
            managerScene = SceneManager.GetSceneByName(Prebuild.kSceneName);
            yield return null;
            var dontDestroyOnLoadObjects = DontDestroyOnLoadObjects(out var lamb);
            foreach (var o in dontDestroyOnLoadObjects)
            {
                runtimeController = o.GetComponent<GameFlowRuntimeController>();
                if (runtimeController != null) break;
            }

            Object.Destroy(lamb);
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

        public static IEnumerator Unload()
        {
            Object.Destroy(root);
            yield return SceneManager.UnloadSceneAsync(managerScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }

        public static IEnumerable<GameObject> DontDestroyOnLoadObjects(out GameObject lamb)
        {
            lamb = new GameObject("Sacrificial Lamb");
            Object.DontDestroyOnLoad(lamb);
            return lamb.scene.GetRootGameObjects();
        }
    }
}
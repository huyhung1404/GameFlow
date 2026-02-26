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
        internal static Scene ManagerScene { get; private set; }
        internal static GameObject Root { get; private set; }
        internal static GameFlowManager Manager { get; private set; }
        internal static GameFlowRuntimeController RuntimeController { get; private set; }
        internal static LoadingController LoadingController { get; private set; }
        internal static DisplayLoading ImageLoading { get; private set; }
        internal static FadeLoading FadeLoading { get; private set; }
        internal static ProgressLoading ProgressLoading { get; private set; }

        public static IEnumerator Load()
        {
            SceneManager.LoadScene(Prebuild.k_SceneBuildPath, LoadSceneMode.Additive);
            ManagerScene = SceneManager.GetSceneByName(Prebuild.k_SceneName);
            yield return null;
            var dontDestroyOnLoadObjects = DontDestroyOnLoadObjects(out var lamb);
            foreach (var o in dontDestroyOnLoadObjects)
            {
                RuntimeController = o.GetComponent<GameFlowRuntimeController>();
                if (RuntimeController != null) break;
            }

            Object.Destroy(lamb);
            Root = RuntimeController.gameObject;
            LoadingController = Root.GetComponentInChildren<LoadingController>();
            ImageLoading = RuntimeController.GetComponentInChildren<DisplayLoading>();
            FadeLoading = RuntimeController.GetComponentInChildren<FadeLoading>();
            ProgressLoading = RuntimeController.GetComponentInChildren<ProgressLoading>();
            const float timeOut = 10;
            var currentTimeOut = 0f;
            while (!RuntimeController.IsActive || currentTimeOut >= timeOut)
            {
                yield return null;
                currentTimeOut += Time.deltaTime;
            }

            if (currentTimeOut >= timeOut) Debug.LogError("Time out!");
            Manager = GameFlowRuntimeController.Manager();
        }

        public static IEnumerator Unload()
        {
            Object.Destroy(Root);
            yield return SceneManager.UnloadSceneAsync(ManagerScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }

        public static IEnumerable<GameObject> DontDestroyOnLoadObjects(out GameObject lamb)
        {
            lamb = new GameObject("Sacrificial Lamb");
            Object.DontDestroyOnLoad(lamb);
            return lamb.scene.GetRootGameObjects();
        }
    }
}
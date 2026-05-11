using System;
using System.Threading.Tasks;
using GameFlow.Component;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GameFlow.Internal
{
    internal static class InstanceManager
    {
        private static Action s_onContextReady;
        private static bool s_isLoading;
        private static GameFlowRuntimeController s_pendingController;
        private static LoadingController s_pendingLoading;
        private static Camera s_pendingCamera;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            s_onContextReady = null;
            s_isLoading = false;
            s_pendingController = null;
            s_pendingLoading = null;
            s_pendingCamera = null;
        }
#endif

        internal static void SetInstance(GameFlowRuntimeController controller)
        {
            var context = GameFlowContext.Current;
            if (context != null)
            {
                context.SetRuntimeController(controller);
                return;
            }

            s_pendingController = controller;
        }

        internal static void SetInstance(LoadingController loading)
        {
            var context = GameFlowContext.Current;
            if (context != null)
            {
                context.SetLoadingController(loading);
                return;
            }

            s_pendingLoading = loading;
        }

        internal static void SetInstance(Camera camera)
        {
            var context = GameFlowContext.Current;
            if (context != null)
            {
                context.SetUICamera(camera);
                return;
            }

            s_pendingCamera = camera;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeOnLoad()
        {
            if (!s_isLoading && GameFlowContext.Current == null)
            {
                _ = LoadManagerAsync(3);
            }
        }

        public static void ConfirmIsInitialized(Action callback)
        {
            if (GameFlowContext.Current != null)
            {
                callback?.Invoke();
                return;
            }

            if (callback != null)
            {
                s_onContextReady += callback;
            }

            if (!s_isLoading)
            {
                _ = LoadManagerAsync(3);
            }
        }

        private static async Task LoadManagerAsync(int maxRetries)
        {
            s_isLoading = true;

            var checkHandle = Addressables.LoadResourceLocationsAsync(PackagePath.ManagerPath());
            await checkHandle.Task;

            if (checkHandle.Status != AsyncOperationStatus.Succeeded || checkHandle.Result.Count == 0)
            {
                Addressables.Release(checkHandle);
                s_isLoading = false;

                s_onContextReady?.Invoke();
                s_onContextReady = null;
                return;
            }

            Addressables.Release(checkHandle);

            var currentTry = 0;

            while (currentTry < maxRetries)
            {
                var loadHandle = Addressables.LoadAssetAsync<GameFlowManager>(PackagePath.ManagerPath());
                await loadHandle.Task;

                if (loadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    var manager = loadHandle.Result;
                    var context = new GameFlowContext(manager);
                    GameFlowContext.SetCurrent(context);

                    s_isLoading = false;

                    ApplyPendingRegistrations(context);

                    if (manager.AutoGenerateRuntimeManager && context.RuntimeController == null)
                    {
                        InitEnvironment(context);
                    }

                    s_onContextReady?.Invoke();
                    s_onContextReady = null;
                    return;
                }

                currentTry++;
                ErrorHandle.LogError($"[{currentTry}/{maxRetries}] Load Game Flow Manager fail at path {PackagePath.ManagerPath()}");
                Addressables.Release(loadHandle);

                if (currentTry < maxRetries)
                {
                    await Task.Delay(1000);
                }
            }

            ErrorHandle.LogError($"Failed to load GameFlowManager after {maxRetries} attempts.");
            s_isLoading = false;

            s_onContextReady?.Invoke();
            s_onContextReady = null;
        }

        private static void ApplyPendingRegistrations(GameFlowContext context)
        {
            if (s_pendingController != null)
            {
                context.SetRuntimeController(s_pendingController);
                s_pendingController = null;
            }

            if (s_pendingLoading != null)
            {
                context.SetLoadingController(s_pendingLoading);
                s_pendingLoading = null;
            }

            if (s_pendingCamera != null)
            {
                context.SetUICamera(s_pendingCamera);
                s_pendingCamera = null;
            }
        }

        private static void InitEnvironment(GameFlowContext context)
        {
            var runtimeControllerObj = new GameObject("[Auto] Flow Controller");
            var controller = runtimeControllerObj.AddComponent<GameFlowRuntimeController>();

            CreateCameras(context, runtimeControllerObj.scene);
            CreateLoadingController(context);

            var elementContainer = new GameObject("Element Container");
            elementContainer.transform.SetParent(controller.transform);

            var uiElementContainer = new GameObject("UI Element Container");
            uiElementContainer.transform.SetParent(controller.transform);

            controller.SetContainer(elementContainer.transform, uiElementContainer.transform);
        }

        private static void CreateCameras(GameFlowContext context, Scene persistentScene)
        {
            var camerasData = context.Manager.AutoGenerateRuntimeManagerData.Cameras;
            if (camerasData == null || camerasData.Length == 0) return;
            var camera = new GameObject("[Auto] Cameras");
            SceneManager.MoveGameObjectToScene(camera, persistentScene);
            foreach (var cameraData in camerasData)
            {
                var instance = Object.Instantiate(cameraData.CameraPrefab, camera.transform);
                if (cameraData.IsMainUICamera) instance.gameObject.AddComponent<FlowUICamera>();
            }
        }

        private static void CreateLoadingController(GameFlowContext context)
        {
            var controller = context.RuntimeController;
            if (controller == null) return;

            var loadingObject = new GameObject("Loading");
            loadingObject.transform.SetParent(controller.transform);
            var loading = loadingObject.AddComponent<LoadingController>();
            loading.gameObject.layer = LayerMask.NameToLayer("UI");

            var loadingData = context.Manager.AutoGenerateRuntimeManagerData.Loadings;
            if (loadingData == null || loadingData.Length == 0)
            {
                loading.SetUp(context.Manager.AutoGenerateRuntimeManagerData.ShieldType, Array.Empty<BaseLoadingTypeController>());
                return;
            }

            var loadingTypeControllers = new BaseLoadingTypeController[loadingData.Length];
            for (var i = loadingData.Length - 1; i >= 0; i--)
            {
                loadingTypeControllers[i] = Object.Instantiate(loadingData[i], loading.transform);
                loadingTypeControllers[i].gameObject.SetActive(false);
                loadingTypeControllers[i].gameObject.layer = LayerMask.NameToLayer("UI");
            }

            loading.SetUp(context.Manager.AutoGenerateRuntimeManagerData.ShieldType, loadingTypeControllers);
        }
    }
}

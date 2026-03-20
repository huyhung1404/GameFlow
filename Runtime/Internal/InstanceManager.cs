using System;
using System.Threading.Tasks;
using GameFlow.Component;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace GameFlow.Internal
{
    internal static class InstanceManager
    {
        internal static GameFlowRuntimeController Instance { get; private set; }
        internal static GameFlowManager Manager { get; private set; }

        private static Action s_onManagerInitialized;
        private static bool s_isLoading;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeOnLoad()
        {
            if (!s_isLoading && Manager == null)
            {
                _ = LoadManagerAsync(3);
            }
        }

        public static void SetInstance(GameFlowRuntimeController runtimeController)
        {
            Instance = runtimeController;
        }

        public static void ConfirmIsInitialized(Action callback)
        {
            if (Manager != null)
            {
                callback?.Invoke();
                return;
            }

            if (callback != null)
            {
                s_onManagerInitialized += callback;
            }

            if (!s_isLoading)
            {
                _ = LoadManagerAsync(3);
            }
        }

        private static async Task LoadManagerAsync(int maxRetries)
        {
            s_isLoading = true;
            var currentTry = 0;

            while (currentTry < maxRetries)
            {
                var operationHandle = Addressables.LoadAssetAsync<GameFlowManager>(PackagePath.ManagerPath());
                await operationHandle.Task;

                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Manager = operationHandle.Result;
                    s_isLoading = false;

                    if (Manager.AutoGenerateRuntimeManager && Instance == null)
                    {
                        InitEnvironment();
                    }

                    s_onManagerInitialized?.Invoke();
                    s_onManagerInitialized = null;
                    return;
                }

                currentTry++;
                ErrorHandle.LogError($"[{currentTry}/{maxRetries}] Load Game Flow Manager fail at path {PackagePath.ManagerPath()}");
                Addressables.Release(operationHandle);

                if (currentTry < maxRetries)
                {
                    await Task.Delay(1000);
                }
            }

            ErrorHandle.LogError($"Failed to load GameFlowManager after {maxRetries} attempts.");
            s_isLoading = false;
        }

        private static void InitEnvironment()
        {
            var runtimeControllerObj = new GameObject("[Auto] Flow Controller");
            Instance = runtimeControllerObj.AddComponent<GameFlowRuntimeController>();

            CreateCameras();
            CreateLoadingControllerInstance();

            var elementContainer = new GameObject("Element Container");
            elementContainer.transform.SetParent(Instance.transform);

            var uiElementContainer = new GameObject("UI Element Container");
            uiElementContainer.transform.SetParent(Instance.transform);

            Instance.SetContainer(elementContainer.transform, uiElementContainer.transform);
        }

        private static void CreateCameras()
        {
            var camerasData = Manager.AutoGenerateRuntimeManagerData.Cameras;
            if (camerasData == null || camerasData.Length == 0) return;
            var camera = new GameObject("[Auto] Cameras");
            Object.DontDestroyOnLoad(camera);
            foreach (var cameraData in camerasData)
            {
                var instance = Object.Instantiate(cameraData.CameraPrefab, camera.transform);
                if (cameraData.IsMainUICamera) instance.gameObject.AddComponent<FlowUICamera>();
            }
        }

        private static void CreateLoadingControllerInstance()
        {
            var loadingObject = new GameObject("Loading");
            loadingObject.transform.SetParent(Instance.transform);
            var loading = loadingObject.AddComponent<LoadingController>();
            loading.gameObject.layer = LayerMask.NameToLayer("UI");

            var loadingData = Manager.AutoGenerateRuntimeManagerData.Loadings;
            if (loadingData == null || loadingData.Length == 0)
            {
                loading.SetUp(Manager.AutoGenerateRuntimeManagerData.ShieldType, Array.Empty<BaseLoadingTypeController>());
                return;
            }

            var loadingTypeControllers = new BaseLoadingTypeController[loadingData.Length];
            for (var i = loadingData.Length - 1; i >= 0; i--)
            {
                loadingTypeControllers[i] = Object.Instantiate(loadingData[i], loading.transform);
                loadingTypeControllers[i].gameObject.SetActive(false);
                loadingTypeControllers[i].gameObject.layer = LayerMask.NameToLayer("UI");
            }

            loading.SetUp(Manager.AutoGenerateRuntimeManagerData.ShieldType, loadingTypeControllers);
        }
    }
}
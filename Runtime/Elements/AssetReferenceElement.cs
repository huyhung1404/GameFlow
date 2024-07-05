using System;
using GameFlow.Internal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameFlow
{
    [Serializable]
    public class AssetReferenceElement : AssetReferenceT<Object>
    {
        [SerializeField] private bool isScene;
        private bool isReleasing;

        #region Editor Setup

        public AssetReferenceElement(string guid) : base(guid)
        {
            isScene = false;
        }

        public AssetReferenceElement(string guid, bool isScene) : base(guid)
        {
            this.isScene = isScene;
        }

        public override bool ValidateAsset(Object obj)
        {
#if UNITY_EDITOR
            return obj is GameObject || obj is SceneAsset;
#else
            return true;
#endif
        }

        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            var type = AssetDatabase.GetMainAssetTypeAtPath(path);
            return type == typeof(GameObject) || type == typeof(SceneAsset);
#else
            return false;
#endif
        }

#if UNITY_EDITOR
        public override bool SetEditorAsset(Object value)
        {
            if (!base.SetEditorAsset(value)) return false;
            isScene = value is SceneAsset;
            return true;
        }

        public override bool SetEditorSubObject(Object value)
        {
            if (!base.SetEditorSubObject(value)) return false;
            isScene = value is SceneAsset;
            return true;
        }
#endif

        #endregion

        internal bool IsReady() => IsDone && !isReleasing;
        internal bool IsScene() => isScene;

        internal void LoadGameObjectHandle(AddCommand command)
        {
            if (isScene)
            {
                var hasActiveHandle = command.activeHandle != null;
                if (hasActiveHandle)
                {
                    HandleReferenceSceneWithActiveHandle(command);
                    return;
                }

                HandleReferenceScene(command);
                return;
            }

            HandleReferencePrefab(command);
        }

        private void HandleReferenceScene(AddCommand command)
        {
            LoadSceneAsync(LoadSceneMode.Additive).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var elementHandle = MoveGameObjectToScene(handle);
                    command.HandleReferencePrefab(elementHandle.gameObject);
                    return;
                }

                Addressables.Release(handle);
                command.HandleReferencePrefab(null);
            };
        }

        private void HandleReferenceSceneWithActiveHandle(AddCommand command)
        {
            LoadSceneAsync(LoadSceneMode.Additive, false).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var elementHandle = MoveGameObjectToScene(handle);
                    command.activeHandle.OnHandleLoadCompleted(handle.Result, elementHandle.gameObject);
                    return;
                }

                Addressables.Release(handle);
                command.HandleReferencePrefab(null);
                command.activeHandle.OnHandleLoadFailed();
            };
        }

        private static SceneElementHandle MoveGameObjectToScene(AsyncOperationHandle<SceneInstance> handle)
        {
            var elementHandle = SceneElementHandle.Create();
            SceneManager.MoveGameObjectToScene(elementHandle.gameObject, handle.Result.Scene);
            elementHandle.GetRootsGameObject();
            return elementHandle;
        }

        private void HandleReferencePrefab(AddCommand command)
        {
            InstantiateAsync(GameFlowRuntimeController.PrefabElementContainer(command is AddUICommand)).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    command.HandleReferencePrefab(handle.Result);
                    return;
                }

                Addressables.Release(handle);
                command.HandleReferencePrefab(null);
            };
        }

        internal void ReleaseHandlePrefab(GameObject handle, IReleaseCompleted completed)
        {
            if (isScene)
            {
                HandleReleaseScene(completed);
                return;
            }

            HandleReleasePrefab(handle, completed);
        }

        private void HandleReleaseScene(IReleaseCompleted completed)
        {
            isReleasing = true;
            Addressables.UnloadSceneAsync(OperationHandle).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    completed.UnloadCompleted(true);
                    isReleasing = false;
                    return;
                }

                isReleasing = false;
                completed.UnloadCompleted(false);
            };
        }

        private void HandleReleasePrefab(GameObject handle, IReleaseCompleted completed)
        {
            if (!Addressables.ReleaseInstance(handle)) Object.Destroy(handle);
            completed.UnloadCompleted(true);
        }
    }
}
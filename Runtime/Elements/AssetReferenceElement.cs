using System;
using GameFlow.Internal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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
        private int instanceCount;

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

        internal GameObject InstanceGameObjectHandle()
        {
            return InstanceGameObjectHandle((GameObject)OperationHandle.Result);
        }

        internal GameObject InstanceGameObjectHandle(GameObject o)
        {
            instanceCount++;
            return Object.Instantiate(o, GameFlowRuntimeController.PrefabElementContainer());
        }

        internal void LoadGameObjectHandle(Action<GameObject> callback)
        {
            if (isScene)
            {
                HandleReferenceScene(callback);
                return;
            }

            HandleReferencePrefab(callback);
        }

        private void HandleReferenceScene(Action<GameObject> callback)
        {
            LoadSceneAsync(LoadSceneMode.Additive).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var elementHandle = SceneElementHandle.Create();
                    SceneManager.MoveGameObjectToScene(elementHandle.gameObject, handle.Result.Scene);
                    elementHandle.GetRootsGameObject();
                    callback.Invoke(elementHandle.gameObject);
                    return;
                }

                Addressables.Release(handle);
                callback.Invoke(null);
            };
        }

        private void HandleReferencePrefab(Action<GameObject> callback)
        {
            LoadAssetAsync<GameObject>().Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    callback.Invoke(InstanceGameObjectHandle(handle.Result));
                    return;
                }

                Addressables.Release(handle);
                callback.Invoke(null);
            };
        }
    }
}
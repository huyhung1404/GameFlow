using System;
using GameFlow.Internal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace GameFlow
{
    public delegate void OnAddCommandCompleted(GameObject handleObject);

    public abstract class AddCommand : Command
    {
        private bool isExecute;
        protected readonly string id;
        internal int loadingId;
        internal bool isPreload;
        internal OnAddCommandCompleted onCompleted;
        internal object sendData;
        private bool isLoadingOn;
        protected abstract GameFlowElement baseElement { get; set; }

        internal AddCommand(Type elementType, string id) : base(elementType)
        {
            isExecute = false;
            isLoadingOn = false;
            this.id = id;
        }

        internal override void Update()
        {
            if (isExecute) return;
            isExecute = Execute();
        }

        private bool Execute()
        {
            try
            {
                if (!GetElementsIfNeed()) return true;
                var reference = baseElement.reference;
                if (!reference.IsDone) return false;
                Loading();
                return true;
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, $"Add Command Error: {elementType.Name}");
                OnLoadResult(null);
                return true;
            }
        }

        private bool GetElementsIfNeed()
        {
            if (baseElement != null) return true;
            var collection = GameFlowRuntimeController.GetElements();
            if (collection.TryGetElement(elementType, id, out var element))
            {
                baseElement = element;
                return true;
            }

            ErrorHandle.LogError($"Element type {elementType.Name} not exits");
            OnLoadResult(null);
            return false;
        }

        private void Loading()
        {
            BaseLoadingTypeController loading = null;
            if (loadingId >= 0) loading = LoadingController.instance.LoadingOn(loadingId);
            if (!loading)
            {
                AddElement();
                return;
            }

            isLoadingOn = true;
            loading.OnCompleted(AddElement);
        }

        private void AddElement()
        {
            if (baseElement.runtimeInstance)
            {
                if (isPreload)
                {
                    OnLoadResult(null);
                    return;
                }

                if (baseElement.runtimeInstance.activeSelf)
                {
                    IsCanReActiveElement();
                    return;
                }

                ActiveElement();
                return;
            }

            if (baseElement.reference.isScene)
            {
                HandleReferenceScene();
                return;
            }

            HandleReferencePrefab();
        }

        private void HandleReferenceScene()
        {
            baseElement.reference.LoadSceneAsync(LoadSceneMode.Additive).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    var elementHandle = SceneElementHandle.Create();
                    baseElement.runtimeInstance = elementHandle.gameObject;
                    SceneManager.MoveGameObjectToScene(baseElement.runtimeInstance, handle.Result.Scene);
                    elementHandle.GetRootsGameObject();
                    ActiveElement();
                    return;
                }

                Addressables.Release(handle);
                OnLoadResult(null);
            };
        }

        private void HandleReferencePrefab()
        {
            baseElement.reference.InstantiateAsync(GameFlowRuntimeController.PrefabElementContainer()).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    baseElement.runtimeInstance = handle.Result;
                    ActiveElement();
                    return;
                }

                Addressables.Release(handle);
                OnLoadResult(null);
            };
        }

        private void IsCanReActiveElement()
        {
            if (!baseElement.canReActive)
            {
                ErrorHandle.LogWarning("Element already exists, to re active please adjust in manager editor");
                OnLoadResult(null);
                return;
            }

            ReActiveElement();
        }

        protected abstract void ReActiveElement();
        protected abstract void ActiveElement();

        protected void OnLoadResult(GameObject result)
        {
            onCompleted?.Invoke(result);
            if (loadingId >= 0 && isLoadingOn) LoadingController.instance.LoadingOff(loadingId);
            Release();
        }
    }
}
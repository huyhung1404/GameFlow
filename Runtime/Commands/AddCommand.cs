using System;
using GameFlow.Internal;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameFlow
{
    public class AddCommand : Command
    {
        private bool isExecute;
        private readonly string id;
        private GameFlowElement element;
        internal int loadingId;
        internal bool isPreload;
        internal OnCommandCompleted onCompleted;

        internal AddCommand(Type elementType, string id) : base(elementType)
        {
            isExecute = false;
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
                var reference = element.reference;
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
            if (element != null) return true;
            var collection = GameFlowRuntimeController.GetElements();
            if (collection.TryGetElement(elementType, id, out element)) return true;
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

            loading.OnCompleted(AddElement);
        }

        private void AddElement()
        {
            if (element.runtimeInstance)
            {
                if (isPreload)
                {
                    OnLoadResult(null);
                    return;
                }

                if (element.runtimeInstance.activeSelf)
                {
                    ReActiveElement();
                    return;
                }

                ActiveElement();
                return;
            }

            element.reference.InstantiateAsync(GameFlowRuntimeController.PrefabElementContainer()).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    element.runtimeInstance = handle.Result;
                    ActiveElement();
                    return;
                }

                Addressables.Release(handle);
                OnLoadResult(null);
            };
        }

        private void ReActiveElement()
        {
            if (!element.canReActive)
            {
                ErrorHandle.LogWarning("Element already exists, to re active please adjust in manager editor");
                OnLoadResult(null);
                return;
            }

            CloseElement();
            ActiveElement();
        }

        private void CloseElement()
        {
            element.runtimeInstance.SetActive(false);
        }

        private void ActiveElement()
        {
            element.runtimeInstance.SetActive(true);
            ElementsRuntimeManager.AddElement(element);
            OnLoadResult(element.runtimeInstance);
        }

        private void OnLoadResult(object result)
        {
            onCompleted?.Invoke(result);
            if (loadingId >= 0) LoadingController.instance.LoadingOff(loadingId);
            Release();
        }
    }
}
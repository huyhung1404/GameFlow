using System;
using GameFlow.Internal;
using UnityEngine;

namespace GameFlow
{
    public delegate void OnAddCommandCompleted(GameObject handleObject);

    public abstract class AddCommand : Command
    {
        private bool isExecute;
        internal int loadingId = -1;
        internal bool isPreload;
        internal OnAddCommandCompleted onCompleted;
        internal object sendData;
        private bool isLoadingOn;
        protected bool callbackOnRelease;

        protected abstract GameFlowElement baseElement { get; set; }

        internal AddCommand(Type elementType) : base(elementType)
        {
            isExecute = false;
            isLoadingOn = false;
            callbackOnRelease = false;
        }

        internal override void PreUpdate()
        {
            var collection = GameFlowRuntimeController.GetElements();
            if (collection.TryGetElement(elementType, out var element))
            {
                baseElement = element;
                return;
            }

            ErrorHandle.LogError($"Element type {elementType.Name} not exits");
            OnLoadResult(null);
            isExecute = true;
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
                var reference = baseElement.reference;
                if (!reference.IsReady()) return false;
                if (baseElement.runtimeInstance)
                {
                    if (isPreload)
                    {
                        OnLoadResult(null);
                        return true;
                    }

                    if (baseElement.runtimeInstance.activeSelf)
                    {
                        IsCanReActiveElement();
                        return true;
                    }

                    ActiveElement();
                    return true;
                }

                if (!reference.IsValid())
                {
                    Loading();
                    return true;
                }

                ErrorHandle.LogWarning($"Reference isValid: {elementType.Name}");
                OnLoadResult(null);
                return true;
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, $"Add Command Error: {elementType.Name}");
                OnLoadResult(null);
                return true;
            }
        }

        private void Loading()
        {
            BaseLoadingTypeController loading = null;
            if (loadingId >= 0) loading = LoadingController.instance.LoadingOn(loadingId);
            if (ReferenceEquals(loading, null))
            {
                baseElement.reference.LoadGameObjectHandle(this);
                return;
            }

            isLoadingOn = true;
            loading.OnCompleted(() => baseElement.reference.LoadGameObjectHandle(this));
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

        internal void HandleReferencePrefab(GameObject handle)
        {
            if (ReferenceEquals(handle, null))
            {
                OnLoadResult(null);
                return;
            }

            baseElement.runtimeInstance = handle;
            ActiveElement();
        }

        protected abstract void ReActiveElement();
        protected abstract void ActiveElement();

        protected void OnLoadResult(GameObject result)
        {
            onCompleted?.Invoke(result);
            if (loadingId >= 0 && isLoadingOn) LoadingController.instance.LoadingOff(loadingId);
            Release();
        }

        internal override void OnRelease()
        {
            if (!callbackOnRelease) return;
            OnLoadResult(baseElement.runtimeInstance);
            if (ReferenceEquals(sendData, null))
            {
                FlowSubject.Event(elementType).RaiseOnActive();
                return;
            }

            FlowSubject.Event(elementType).RaiseOnActiveWithData(sendData);
        }
    }
}
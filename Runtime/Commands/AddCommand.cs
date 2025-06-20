﻿using System;
using GameFlow.Internal;
using UnityEngine;

namespace GameFlow
{
    public delegate void OnAddCommandCompleted(GameObject handleObject);

    public abstract class AddCommand : Command
    {
        internal int loadingId = -1;
        internal bool isPreload;
        internal object activeData;
        internal OnAddCommandCompleted onCompleted;
        internal ReferenceActiveHandle activeHandle;
        protected bool callbackOnRelease;
        private bool isExecute;
        private bool isLoadingOn;

        protected abstract GameFlowElement baseElement { get; set; }
        internal ElementReleaseMode ReleaseMode() => baseElement.releaseMode;

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
                activeHandle?.SetReference(element.reference);
                return;
            }

            ErrorHandle.LogError($"Element type {elementType.Name} not exists");
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
                        HandleActiveMode();
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

        private void HandleActiveMode()
        {
            switch (baseElement.activeMode)
            {
                default:
                case ElementActiveMode.SINGLETON:
                    ErrorHandle.LogWarning("Element already exists, to re active please adjust in manager editor");
                    OnLoadResult(null);
                    return;
                case ElementActiveMode.RE_ACTIVE:
                    ReActiveElement();
                    return;
                case ElementActiveMode.MULTI_INSTANCE:
                    new CloneCommand(elementType, this).BuildClone();
                    if (loadingId >= 0 && isLoadingOn) LoadingController.instance.LoadingOff(loadingId);
                    Release();
                    return;
            }
        }

        internal void HandleReferencePrefab(GameObject handle)
        {
            if (ReferenceEquals(handle, null))
            {
                OnLoadResult(null);
                return;
            }

            baseElement.runtimeInstance = handle;
            if (isPreload)
            {
                baseElement.runtimeInstance.SetActive(false);
                OnLoadResult(baseElement.runtimeInstance);
                return;
            }

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
            var delegates = FlowObservable.Event(elementType);
            if (!ReferenceEquals(activeData, null)) delegates.RaiseOnActiveWithData(activeData);
            delegates.RaiseOnActive();
        }

        internal override string GetFullInfo()
        {
            return $@"<b><size=11>isRelease:</size></b> {isRelease}
<b><size=11>loadingId:</size></b> {loadingId}
<b><size=11>isPreload:</size></b> {isPreload}
<b><size=11>activeData:</size></b> {activeData}
<b><size=11>onCompleted:</size></b> {onCompleted?.Target}.{onCompleted?.Method.Name}
<b><size=11>activeHandle:</size></b> {activeHandle}
<b><size=11>callbackOnRelease:</size></b> {callbackOnRelease}
<b><size=11>isExecute:</size></b> {isExecute}
<b><size=11>isLoadingOn:</size></b> {isLoadingOn}
<b><size=11>isUserInterface:</size></b> {this is AddUICommand}";
        }
    }
}
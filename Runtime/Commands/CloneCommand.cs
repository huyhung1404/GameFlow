using System;
using GameFlow.Internal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameFlow
{
    internal class CloneCommand : Command
    {
        private readonly AddCommand baseCommand;
        private bool callbackOnRelease;
        private bool isExecute;
        private bool isLoadingOn;
        private CloneElement clone;

        public CloneCommand(Type elementType, AddCommand baseCommand) : base(elementType)
        {
            this.baseCommand = baseCommand;
            isExecute = false;
            isLoadingOn = false;
            callbackOnRelease = false;
        }

        internal override void PreUpdate()
        {
            var collection = GameFlowRuntimeController.GetElements();
            if (collection.TryGetElement(elementType, out var element))
            {
                clone = element is UIFlowElement uiFlow ? new UICloneElement(uiFlow) : new CloneFlowElement(element);
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
                var reference = clone.CloneElementInstance().reference;
                if (!reference.IsReady()) return false;
                if (reference.IsScene())
                {
                    OnLoadResult(null);
                    ErrorHandle.LogError("Multi Instance Not Support Scene!");
                    return true;
                }

                Loading();
                return true;
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, $"Clone Command Error: {elementType.Name}");
                OnLoadResult(null);
                return true;
            }
        }

        private void Loading()
        {
            BaseLoadingTypeController loading = null;
            if (baseCommand.loadingId >= 0) loading = LoadingController.instance.LoadingOn(baseCommand.loadingId);
            if (ReferenceEquals(loading, null))
            {
                HandleReferencePrefab();
                return;
            }

            isLoadingOn = true;
            loading.OnCompleted(HandleReferencePrefab);
        }

        private void HandleReferencePrefab()
        {
            clone.CloneElementInstance().reference.InstantiateAsync(GameFlowRuntimeController.PrefabElementContainer(baseCommand is AddUICommand)).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    HandleReferencePrefab(handle.Result);
                    return;
                }

                Addressables.Release(handle);
                HandleReferencePrefab(null);
            };
        }

        private void HandleReferencePrefab(GameObject handle)
        {
            if (ReferenceEquals(handle, null))
            {
                OnLoadResult(null);
                return;
            }

            clone.CloneElementInstance().runtimeInstance = handle;
            clone.ReplaceElement();
            clone.ActiveElement();
            callbackOnRelease = true;
            Release();
        }

        protected void OnLoadResult(GameObject result)
        {
            baseCommand.onCompleted?.Invoke(result);
            if (baseCommand.loadingId >= 0 && isLoadingOn) LoadingController.instance.LoadingOff(baseCommand.loadingId);
            Release();
        }

        internal override void OnRelease()
        {
            if (!callbackOnRelease) return;
            OnLoadResult(clone.RuntimeInstance());
            var delegates = FlowObservable.Event(elementType);
            if (!ReferenceEquals(baseCommand.activeData, null)) delegates.RaiseOnActiveWithData(baseCommand.activeData);
            delegates.RaiseOnActive();
        }

        internal override string GetFullInfo()
        {
            return $@"<b><size=11>Is Clone</size></b>
<b><size=11>isRelease:</size></b> {isRelease}
<b><size=11>loadingId:</size></b> {baseCommand.loadingId}
<b><size=11>activeData:</size></b> {baseCommand.activeData}
<b><size=11>onCompleted:</size></b> {baseCommand.onCompleted?.Target}.{baseCommand.onCompleted?.Method.Name}
<b><size=11>callbackOnRelease:</size></b> {callbackOnRelease}
<b><size=11>isExecute:</size></b> {isExecute}
<b><size=11>isLoadingOn:</size></b> {isLoadingOn}
<b><size=11>isUserInterface:</size></b> {baseCommand is AddUICommand}";
        }

        internal void BuildClone()
        {
            GameFlowRuntimeController.OverriderCommand(this);
        }
    }
}
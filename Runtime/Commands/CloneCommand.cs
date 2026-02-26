using System;
using GameFlow.Internal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameFlow
{
    internal class CloneCommand : Command
    {
        private readonly AddCommand _baseCommand;
        private bool _callbackOnRelease;
        private bool _isExecute;
        private bool _isLoadingOn;
        private CloneElement _clone;

        public CloneCommand(Type elementType, AddCommand baseCommand) : base(elementType)
        {
            _baseCommand = baseCommand;
            _isExecute = false;
            _isLoadingOn = false;
            _callbackOnRelease = false;
        }

        internal override void PreUpdate()
        {
            var collection = GameFlowRuntimeController.GetElements();
            if (collection.TryGetElement(_elementType, out var element))
            {
                _clone = element is UIFlowElement uiFlow ? new UICloneElement(uiFlow) : new CloneFlowElement(element);
                return;
            }

            ErrorHandle.LogError($"Element type {_elementType.Name} not exists");
            OnLoadResult(null);
            _isExecute = true;
        }

        internal override void Update()
        {
            if (_isExecute) return;
            _isExecute = Execute();
        }

        private bool Execute()
        {
            try
            {
                var reference = _clone.CloneElementInstance().Reference;
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
                ErrorHandle.LogException(e, $"Clone Command Error: {_elementType.Name}");
                OnLoadResult(null);
                return true;
            }
        }

        private void Loading()
        {
            BaseLoadingTypeController loading = null;
            if (_baseCommand.LoadingId >= 0) loading = LoadingController.Instance.LoadingOn(_baseCommand.LoadingId);
            if (ReferenceEquals(loading, null))
            {
                HandleReferencePrefab();
                return;
            }

            _isLoadingOn = true;
            loading.OnCompleted(HandleReferencePrefab);
        }

        private void HandleReferencePrefab()
        {
            _clone.CloneElementInstance().Reference.InstantiateAsync(GameFlowRuntimeController.PrefabElementContainer(_baseCommand is AddUICommand)).Completed += handle =>
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

            _clone.CloneElementInstance().RuntimeInstance = handle;
            _clone.ReplaceElement();
            _clone.ActiveElement();
            _callbackOnRelease = true;
            Release();
        }

        protected void OnLoadResult(GameObject result)
        {
            _baseCommand.OnCompleted?.Invoke(result);
            if (_baseCommand.LoadingId >= 0 && _isLoadingOn) LoadingController.Instance.LoadingOff(_baseCommand.LoadingId);
            Release();
        }

        internal override void OnRelease()
        {
            if (!_callbackOnRelease) return;
            OnLoadResult(_clone.RuntimeInstance());
            var delegates = FlowObservable.Event(_elementType);
            if (!ReferenceEquals(_baseCommand.ActiveData, null)) delegates.RaiseOnActiveWithData(_baseCommand.ActiveData);
            delegates.RaiseOnActive();
        }

        internal override string GetFullInfo()
        {
            return $@"<b><size=11>Is Clone</size></b>
<b><size=11>isRelease:</size></b> {IsRelease}
<b><size=11>loadingId:</size></b> {_baseCommand.LoadingId}
<b><size=11>activeData:</size></b> {_baseCommand.ActiveData}
<b><size=11>onCompleted:</size></b> {_baseCommand.OnCompleted?.Target}.{_baseCommand.OnCompleted?.Method.Name}
<b><size=11>callbackOnRelease:</size></b> {_callbackOnRelease}
<b><size=11>isExecute:</size></b> {_isExecute}
<b><size=11>isLoadingOn:</size></b> {_isLoadingOn}
<b><size=11>isUserInterface:</size></b> {_baseCommand is AddUICommand}";
        }

        internal void BuildClone()
        {
            GameFlowRuntimeController.OverriderCommand(this);
        }
    }
}
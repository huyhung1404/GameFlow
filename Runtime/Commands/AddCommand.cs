using System;
using GameFlow.Internal;
using UnityEngine;

namespace GameFlow
{
    public delegate void OnAddCommandCompleted(GameObject handleObject);

    public abstract class AddCommand : Command
    {
        internal LoadingId? LoadingId;
        internal bool IsPreload;
        internal bool ActiveSceneOnLoadCompleted = true;
        internal OnAddCommandCompleted OnCompleted;
        internal ReferenceActiveHandle ActiveHandle;
        protected bool _callbackOnRelease;
        private bool _isExecute;
        private bool _isLoadingOn;

        protected abstract GameFlowElement BaseElement { get; set; }
        internal ElementReleaseMode ReleaseMode() => BaseElement.ReleaseMode;

        internal AddCommand(Type elementType) : base(elementType)
        {
            _isExecute = false;
            _isLoadingOn = false;
            _callbackOnRelease = false;
        }

        internal override void PreUpdate()
        {
            var collection = Context.RuntimeController.GetElements();
            if (collection.TryGetElement(_elementType, out var element))
            {
                BaseElement = element;
                ActiveHandle?.SetReference(element.Reference);
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
                var reference = BaseElement.Reference;
                if (!reference.IsReady()) return false;
                if (BaseElement.RuntimeInstance)
                {
                    if (IsPreload)
                    {
                        OnLoadResult(null);
                        return true;
                    }

                    if (BaseElement.RuntimeInstance.activeSelf)
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

                ErrorHandle.LogWarning($"Reference isValid: {_elementType.Name}");
                OnLoadResult(null);
                return true;
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, $"Add Command Error: {_elementType.Name}");
                OnLoadResult(null);
                return true;
            }
        }

        private void Loading()
        {
            var loading = Context.Loading;
            BaseLoadingTypeController loadingController = null;
            if (LoadingId.HasValue && loading != null) loadingController = loading.LoadingOn(LoadingId.Value);
            if (ReferenceEquals(loadingController, null))
            {
                BaseElement.Reference.LoadGameObjectHandle(this);
                return;
            }

            _isLoadingOn = true;
            loadingController.OnCompleted(() => BaseElement.Reference.LoadGameObjectHandle(this));
        }

        private void HandleActiveMode()
        {
            switch (BaseElement.ActiveMode)
            {
                default:
                case ElementActiveMode.Singleton:
                    ErrorHandle.LogWarning("Element already exists, to re active please adjust in manager editor");
                    OnLoadResult(null);
                    return;
                case ElementActiveMode.ReActive:
                    ReActiveElement();
                    return;
                case ElementActiveMode.MultiInstance:
                    new CloneCommand(_elementType, this).BuildClone();
                    if (LoadingId.HasValue && _isLoadingOn && Context.Loading != null)
                        Context.Loading.LoadingOff(LoadingId.Value);
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

            BaseElement.RuntimeInstance = handle;
            if (IsPreload)
            {
                BaseElement.RuntimeInstance.SetActive(false);
                OnLoadResult(BaseElement.RuntimeInstance);
                return;
            }

            ActiveElement();
        }

        internal virtual void RaiseActiveData(ElementCallbackEvent delegates) { }
        internal virtual string GetActiveDataInfo() => "null";
        protected abstract void ReActiveElement();
        protected abstract void ActiveElement();

        protected void OnLoadResult(GameObject result)
        {
            OnCompleted?.Invoke(result);
            if (LoadingId.HasValue && _isLoadingOn && Context.Loading != null)
                Context.Loading.LoadingOff(LoadingId.Value);
            Release();
        }

        internal override void OnRelease()
        {
            if (!_callbackOnRelease) return;
            OnLoadResult(BaseElement.RuntimeInstance);
            var delegates = BaseElement.CallbackEvent;
            RaiseActiveData(delegates);
            delegates.RaiseOnActive();
        }

        internal override string GetFullInfo()
        {
            return $@"<b><size=11>isRelease:</size></b> {IsRelease}
<b><size=11>loadingId:</size></b> {LoadingId}
<b><size=11>isPreload:</size></b> {IsPreload}
<b><size=11>activeData:</size></b> {GetActiveDataInfo()}
<b><size=11>onCompleted:</size></b> {OnCompleted?.Target}.{OnCompleted?.Method.Name}
<b><size=11>activeHandle:</size></b> {ActiveHandle}
<b><size=11>activeSceneOnLoadCompleted:</size></b> {ActiveSceneOnLoadCompleted}
<b><size=11>callbackOnRelease:</size></b> {_callbackOnRelease}
<b><size=11>isExecute:</size></b> {_isExecute}
<b><size=11>isLoadingOn:</size></b> {_isLoadingOn}
<b><size=11>isUserInterface:</size></b> {this is AddUICommand}";
        }
    }

}

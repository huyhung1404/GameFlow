using System;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace GameFlow
{
    public enum ActiveHandleStatus
    {
        None,
        Succeeded,
        Failed
    }

    public class ReferenceActiveHandle
    {
        private AssetReferenceElement _reference;
        protected readonly AddCommand _command;
        protected ActiveHandleStatus _status;
        protected SceneInstance _resultInstance;
        protected GameObject _elementHandle;
        protected Action<SceneInstance> _onCompleted;
        private Action<ActiveHandleStatus> _onLoadResult;

        public event Action<ActiveHandleStatus> OnLoadResult { add => _onLoadResult += value; remove => _onLoadResult -= value; }
        public event Action<SceneInstance> OnCompleted { add => _onCompleted += value; remove => _onCompleted -= value; }

        internal ReferenceActiveHandle(AddCommand command)
        {
            _status = ActiveHandleStatus.None;
            _command = command;
        }

        internal void SetReference(AssetReferenceElement referenceElement)
        {
            _reference = referenceElement;
        }

        internal void OnHandleLoadCompleted(SceneInstance sceneInstance, GameObject elementHandleObject)
        {
            _resultInstance = sceneInstance;
            _elementHandle = elementHandleObject;
            _status = ActiveHandleStatus.Succeeded;
            _onLoadResult?.Invoke(_status);
        }

        internal void OnHandleLoadFailed()
        {
            _status = ActiveHandleStatus.Failed;
            _onLoadResult?.Invoke(_status);
        }

        public virtual bool ActiveScene()
        {
            if (_status != ActiveHandleStatus.Succeeded) return false;
            _resultInstance.ActivateAsync().completed += _ =>
            {
                if (_command.ActiveSceneOnLoadCompleted)
                    SceneManager.SetActiveScene(_resultInstance.Scene);
                _onCompleted?.Invoke(_resultInstance);
                _onCompleted = null;
                _command.HandleReferencePrefab(_elementHandle);
            };

            return true;
        }

        public float Progress()
        {
            if (_reference == null) return 0;
            if (!_reference.OperationHandle.IsValid()) return 0;
            if (_reference.OperationHandle.IsDone) return 1;
            return _reference.OperationHandle.PercentComplete;
        }

        public bool IsDone()
        {
            return _reference != null && _reference.OperationHandle.IsDone;
        }

        public override string ToString()
        {
            return $"[Status:{_status}] [Scene:{_resultInstance.Scene.name}] [Handle:{_elementHandle?.name}] " +
                   $"[OnCompeted:{_onCompleted?.Target}.{_onCompleted?.Method.Name}] [OnLoadResult:{_onLoadResult?.Target}.{_onLoadResult?.Method}]";
        }
    }
}

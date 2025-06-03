using System;
using GameFlow.Internal;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

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
        private AssetReferenceElement reference;
        protected readonly AddCommand command;
        protected ActiveHandleStatus status;
        protected SceneInstance resultInstance;
        protected GameObject elementHandle;
        protected Action<SceneInstance> onCompleted;
        private Action<ActiveHandleStatus> onLoadResult;

        public event Action<ActiveHandleStatus> OnLoadResult { add => onLoadResult += value; remove => onLoadResult -= value; }
        public event Action<SceneInstance> OnCompleted { add => onCompleted += value; remove => onCompleted -= value; }

        internal ReferenceActiveHandle(AddCommand command)
        {
            status = ActiveHandleStatus.None;
            this.command = command;
        }

        internal void SetReference(AssetReferenceElement referenceElement)
        {
            reference = referenceElement;
        }

        internal void OnHandleLoadCompleted(SceneInstance sceneInstance, GameObject elementHandleObject)
        {
            resultInstance = sceneInstance;
            elementHandle = elementHandleObject;
            status = ActiveHandleStatus.Succeeded;
            onLoadResult?.Invoke(status);
        }

        internal void OnHandleLoadFailed()
        {
            status = ActiveHandleStatus.Failed;
            onLoadResult?.Invoke(status);
        }

        public virtual bool ActiveScene()
        {
            if (status != ActiveHandleStatus.Succeeded) return false;
            resultInstance.ActivateAsync().completed += _ =>
            {
                onCompleted?.Invoke(resultInstance);
                onCompleted = null;
                command.HandleReferencePrefab(elementHandle);
            };

            return true;
        }

        public float Progress()
        {
            if (reference == null) return 0;
            if (!reference.OperationHandle.IsValid()) return 0;
            if (reference.OperationHandle.IsDone) return 1;
            return reference.OperationHandle.PercentComplete;
        }

        public bool IsDone()
        {
            return reference != null && reference.OperationHandle.IsDone;
        }

        public override string ToString()
        {
            return $"[Status:{status}] [Scene:{resultInstance.Scene.name}] [Handle:{elementHandle?.name}] " +
                   $"[OnCompeted:{onCompleted?.Target}.{onCompleted?.Method.Name}] [OnLoadResult:{onLoadResult?.Target}.{onLoadResult?.Method}]";
        }
    }

    public class ReferenceActiveHandleForLoadCommand : ReferenceActiveHandle
    {
        internal ReferenceActiveHandleForLoadCommand(AddCommand command) : base(command)
        {
        }

        public override bool ActiveScene()
        {
            if (status != ActiveHandleStatus.Succeeded) return false;
            UIElementsRuntimeManager.ReleaseAllElement(() => base.ActiveScene());
            return true;
        }
    }
}
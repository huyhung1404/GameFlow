using System;
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
        private readonly AddCommand command;
        private ActiveHandleStatus status;
        private SceneInstance resultInstance;
        private GameObject elementHandle;
        private Action<ActiveHandleStatus> onLoadResult;
        public event Action<ActiveHandleStatus> OnLoadResult { add => onLoadResult += value; remove => onLoadResult -= value; }

        internal ReferenceActiveHandle(AddCommand command)
        {
            status = ActiveHandleStatus.None;
            this.command = command;
        }

        internal void OnHandleLoadCompleted(SceneInstance sceneInstance, GameObject elementHandleObject)
        {
            status = ActiveHandleStatus.Succeeded;
            resultInstance = sceneInstance;
            elementHandle = elementHandleObject;
            onLoadResult?.Invoke(status);
        }

        internal void OnHandleLoadFailed()
        {
            status = ActiveHandleStatus.Failed;
            onLoadResult?.Invoke(status);
        }

        public void ActiveScene(Action onCompleted = null)
        {
            if (status != ActiveHandleStatus.Succeeded) return;
            resultInstance.ActivateAsync().completed += _ =>
            {
                onCompleted?.Invoke();
                command.HandleReferencePrefab(elementHandle);
            };
        }
    }
}
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

        public bool ActiveScene(Action onCompleted = null)
        {
            if (status != ActiveHandleStatus.Succeeded) return false;
            resultInstance.ActivateAsync().completed += _ =>
            {
                onCompleted?.Invoke();
                command.HandleReferencePrefab(elementHandle);
            };

            return true;
        }
    }
}
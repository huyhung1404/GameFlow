using System;
using UnityEngine;

namespace GameFlow.Internal
{
    internal class LoadingController : MonoBehaviour
    {
        [SerializeField] private BaseLoadingTypeController[] controllers = Array.Empty<BaseLoadingTypeController>();
        private int totalController;

        private void Awake()
        {
            totalController = controllers.Length;
        }

        public void RegisterControllers(params BaseLoadingTypeController[] registerControllers)
        {
            totalController += registerControllers.Length;
            var mergedArray = new BaseLoadingTypeController[totalController];
            controllers.CopyTo(mergedArray, 0);
            registerControllers.CopyTo(mergedArray, totalController);
            controllers = mergedArray;
        }

        internal bool IsShow()
        {
            for (var i = 0; i < totalController; i++)
            {
                if (controllers[i].isShow) return true;
            }

            return false;
        }

        internal BaseLoadingTypeController LoadingOn(int i)
        {
            if (i < controllers.Length) return controllers[i].On();
            ErrorHandle.LogError($"Loading controller not exits id {i}");
            return null;
        }

        internal BaseLoadingTypeController LoadingOff(int i)
        {
            if (i < controllers.Length) return controllers[i].Off();
            ErrorHandle.LogError($"Loading controller not exits id {i}");
            return null;
        }

        private void LateUpdate()
        {
            for (var i = 0; i < totalController; i++)
            {
                var typeController = controllers[i];
                if (typeController.isShow == typeController.isEnable) continue;
                typeController.gameObject.SetActive(typeController.isShow);
            }
        }
    }
}
using System;
using UnityEngine;

namespace GameFlow.Internal
{
    internal class LoadingController : MonoBehaviour
    {
        private static LoadingController instance;
        [SerializeField] private BaseLoadingTypeController[] controllers = Array.Empty<BaseLoadingTypeController>();
        private static int totalController;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            totalController = controllers.Length;
            DontDestroyOnLoad(this);
        }

        public void RegisterControllers(params BaseLoadingTypeController[] registerControllers)
        {
            var mergedArray = new BaseLoadingTypeController[totalController + registerControllers.Length];
            controllers.CopyTo(mergedArray, 0);
            registerControllers.CopyTo(mergedArray, totalController);
            controllers = mergedArray;
            totalController = controllers.Length;
        }

        internal static bool IsShow()
        {
            for (var i = 0; i < totalController; i++)
            {
                if (instance.controllers[i].isShow) return true;
            }

            return false;
        }

        internal BaseLoadingTypeController LoadingOn(int i)
        {
            if (i < totalController) return controllers[i].On();
            ErrorHandle.LogError($"Loading controller not exits id {i}");
            return null;
        }

        internal BaseLoadingTypeController LoadingOff(int i)
        {
            if (i < totalController) return controllers[i].Off();
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
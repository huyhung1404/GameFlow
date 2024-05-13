using UnityEngine;

namespace GameFlow.Internal
{
    internal class LoadingController : MonoBehaviour
    {
        [SerializeField] private BaseLoadingTypeController[] controllers;
        private int totalController;

        internal void OverriderControllers(BaseLoadingTypeController[] overriderController)
        {
            controllers = overriderController;
            totalController = overriderController.Length;
        }

        private void Awake()
        {
            totalController = controllers?.Length ?? 0;
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
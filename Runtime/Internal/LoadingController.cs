using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameFlow.Internal
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(Image))]
    internal class LoadingController : MonoBehaviour
    {
#if UNITY_EDITOR
        internal static LoadingController instance { get; set; }
#else
        internal static LoadingController instance;
#endif

        [SerializeField] private BaseLoadingTypeController[] controllers = Array.Empty<BaseLoadingTypeController>();
        private static int totalController;
        private static bool transparentEnable;
        private Image transparent;

        private void Awake()
        {
            instance = this;
            totalController = controllers.Length;
            transparent = GetComponent<Image>();
            transparent.raycastTarget = true;
            transparent.enabled = transparentEnable;
        }

        internal void SetUpShieldSortingOrder(int sortingOrder)
        {
            GetComponent<Canvas>().sortingOrder = sortingOrder;
        }

        internal static void EnableTransparent()
        {
            if (transparentEnable) return;
            instance.transparent.enabled = true;
            transparentEnable = true;
        }

        internal void IsTransparentOff()
        {
            Assert.IsTrue(!transparentEnable);
        }

        internal void IsTransparentOn()
        {
            Assert.IsTrue(transparentEnable);
        }

        internal static void DisableTransparent()
        {
            if (!transparentEnable) return;
            instance.transparent.enabled = false;
            transparentEnable = false;
        }

        public void RegisterControllers(bool isAppend, params BaseLoadingTypeController[] registerControllers)
        {
            if (!isAppend)
            {
                controllers = registerControllers;
                totalController = controllers.Length;
                return;
            }

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
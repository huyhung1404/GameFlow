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
        internal static LoadingController Instance { get; set; }

        [SerializeField] private BaseLoadingTypeController[] m_controllers = Array.Empty<BaseLoadingTypeController>();
        internal static bool s_IsInitialization;
        private static int s_totalController;
        private static bool s_transparentEnable;
        private Image _transparent;

        private void Awake()
        {
            Instance = this;
            s_IsInitialization = true;
            s_totalController = m_controllers.Length;
            _transparent = GetComponent<Image>();
            _transparent.raycastTarget = true;
            _transparent.enabled = s_transparentEnable;
        }

        internal void SetUpShieldSortingOrder(int sortingOrder)
        {
            GetComponent<Canvas>().sortingOrder = sortingOrder;
        }

        internal static void EnableTransparent()
        {
            if (s_transparentEnable) return;
            Instance._transparent.enabled = true;
            s_transparentEnable = true;
        }

        internal void IsTransparentOff()
        {
            Assert.IsTrue(!s_transparentEnable);
        }

        internal void IsTransparentOn()
        {
            Assert.IsTrue(s_transparentEnable);
        }

        internal static void DisableTransparent()
        {
            if (!s_transparentEnable) return;
            Instance._transparent.enabled = false;
            s_transparentEnable = false;
        }

        public void RegisterControllers(bool isAppend, params BaseLoadingTypeController[] registerControllers)
        {
            if (!isAppend)
            {
                m_controllers = registerControllers;
                s_totalController = m_controllers.Length;
                return;
            }

            var mergedArray = new BaseLoadingTypeController[s_totalController + registerControllers.Length];
            m_controllers.CopyTo(mergedArray, 0);
            registerControllers.CopyTo(mergedArray, s_totalController);
            m_controllers = mergedArray;
            s_totalController = m_controllers.Length;
        }

        internal static bool IsShow()
        {
            for (var i = 0; i < s_totalController; i++)
            {
                if (Instance.m_controllers[i].IsShow) return true;
            }

            return false;
        }

        internal BaseLoadingTypeController LoadingOn(int i)
        {
            if (i < s_totalController) return m_controllers[i].On();
            ErrorHandle.LogError($"Loading controller not exists id {i}");
            return null;
        }

        internal BaseLoadingTypeController LoadingOff(int i)
        {
            if (i < s_totalController) return m_controllers[i].Off();
            ErrorHandle.LogError($"Loading controller not exists id {i}");
            return null;
        }

        internal BaseLoadingTypeController Get(int i)
        {
            return i < s_totalController ? m_controllers[i] : null;
        }

        private void LateUpdate()
        {
            for (var i = 0; i < s_totalController; i++)
            {
                var typeController = m_controllers[i];
                if (typeController.IsShow == typeController.IsEnable) continue;
                typeController.gameObject.SetActive(typeController.IsShow);
            }
        }
    }
}
using System;
using UnityEngine;

namespace GameFlow.Internal
{
    internal struct LoadingId
    {
        public readonly string Id;
        public readonly int Index;

        internal LoadingId(string id)
        {
            Id = id;
            Index = -1;
        }

        internal LoadingId(int index)
        {
            Id = null;
            Index = index;
        }
    }

    [AddComponentMenu("Game Flow/Loading Controller")]
    internal class LoadingController : MonoBehaviour
    {
        internal static LoadingController Instance { get; set; }

        [SerializeField] private BaseLoadingTypeController[] m_controllers;
        [SerializeField] private ShieldType m_shieldType;
        internal static bool s_IsInitialization;
        private static int s_totalController;
        private static LoadingShield s_shieldInstance;

        private void Awake()
        {
            Instance = this;
            s_IsInitialization = true;
            s_totalController = m_controllers.Length;
            s_shieldInstance = m_shieldType switch
            {
                ShieldType.UIImage => gameObject.AddComponent<UIImageShield>(),
                _ => throw new ArgumentOutOfRangeException()
            };
            s_shieldInstance.SetUp();
        }

        internal void SetUpShieldSortingOrder(int sortingOrder)
        {
            GetComponent<Canvas>().sortingOrder = sortingOrder;
        }

        internal static void EnableShield()
        {
            s_shieldInstance.OpenShield();
        }

        internal static void IsShieldOff()
        {
            Assert.IsTrue(!s_shieldInstance.IsShieldEnabled);
        }

        internal static void IsShieldOn()
        {
            Assert.IsTrue(s_shieldInstance.IsShieldEnabled);
        }

        internal static void DisableShield()
        {
            s_shieldInstance.CloseShield();
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

        internal BaseLoadingTypeController LoadingOn(LoadingId id)
        {
            if (id.Index < 0)
            {
                foreach (var controller in m_controllers)
                {
                    if (controller.ID == id.Id) return controller.On();
                }

                ErrorHandle.LogError($"Loading controller not exists id {id.Id}");
                return null;
            }

            if (id.Index < s_totalController) return m_controllers[id.Index].On();
            ErrorHandle.LogError($"Loading controller not exists index {id.Index}");
            return null;
        }

        internal BaseLoadingTypeController LoadingOff(LoadingId id)
        {
            if (id.Index < 0)
            {
                foreach (var controller in m_controllers)
                {
                    if (controller.ID == id.Id) return controller.Off();
                }

                ErrorHandle.LogError($"Loading controller not exists id {id.Id}");
                return null;
            }

            if (id.Index < s_totalController) return m_controllers[id.Index].Off();
            ErrorHandle.LogError($"Loading controller not exists index {id.Index}");
            return null;
        }

        internal BaseLoadingTypeController Get(LoadingId id)
        {
            if (id.Index >= 0) return id.Index < s_totalController ? m_controllers[id.Index] : null;
            for (var i = m_controllers.Length - 1; i >= 0; i--)
            {
                var controller = m_controllers[i];
                if (controller.ID == id.Id) return controller;
            }

            return null;
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
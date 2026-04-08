using UnityEngine;

namespace GameFlow.Internal
{
    [AddComponentMenu("Game Flow/Loading Controller")]
    internal class LoadingController : MonoBehaviour
    {
        [SerializeField] private BaseLoadingTypeController[] m_controllers;
        [SerializeField] private ShieldType m_shieldType;
        private int _totalController;
        private LoadingShield _shieldInstance;

        private void Awake()
        {
            var context = GameFlowContext.Current;
            if (context != null)
                context.SetLoadingController(this);
            else
                InstanceManager.SetInstance(this);

            if (m_controllers == null) return;
            SetUp(m_shieldType, m_controllers);
        }

        public void SetUp(ShieldType shieldType, BaseLoadingTypeController[] controllers)
        {
            m_shieldType = shieldType;
            m_controllers = controllers;

            _totalController = m_controllers.Length;
            _shieldInstance = m_shieldType switch
            {
                ShieldType.CanvasOverlay => gameObject.AddComponent<CanvasOverlayShield>(),
                ShieldType.CanvasCamera => gameObject.AddComponent<CanvasCameraShield>(),
                _ => FallbackShield()
            };
            
            _shieldInstance.SetUp();
            GameFlowContext.Current?.SetLoadingController(this);
        }

        private LoadingShield FallbackShield()
        {
            ErrorHandle.LogError($"Unknown ShieldType '{m_shieldType}', falling back to CanvasOverlay.");
            return gameObject.AddComponent<CanvasOverlayShield>();
        }

        internal void SetUpShieldSortingOrder(int sortingOrder)
        {
            GetComponent<Canvas>().sortingOrder = sortingOrder;
        }

        internal void EnableShield()
        {
            _shieldInstance?.OpenShield();
        }

        internal void IsShieldOff()
        {
            Assert.IsTrue(_shieldInstance == null || !_shieldInstance.IsShieldEnabled);
        }

        internal void IsShieldOn()
        {
            Assert.IsTrue(_shieldInstance != null && _shieldInstance.IsShieldEnabled);
        }

        internal void DisableShield()
        {
            _shieldInstance?.CloseShield();
        }

        public void RegisterControllers(bool isAppend, params BaseLoadingTypeController[] registerControllers)
        {
            if (!isAppend)
            {
                m_controllers = registerControllers;
                _totalController = m_controllers.Length;
                return;
            }

            var mergedArray = new BaseLoadingTypeController[_totalController + registerControllers.Length];
            m_controllers.CopyTo(mergedArray, 0);
            registerControllers.CopyTo(mergedArray, _totalController);
            m_controllers = mergedArray;
            _totalController = m_controllers.Length;
        }

        internal bool IsShow()
        {
            for (var i = 0; i < _totalController; i++)
            {
                if (m_controllers[i].IsShow) return true;
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

            if (id.Index < _totalController) return m_controllers[id.Index].On();
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

            if (id.Index < _totalController) return m_controllers[id.Index].Off();
            ErrorHandle.LogError($"Loading controller not exists index {id.Index}");
            return null;
        }

        internal BaseLoadingTypeController Get(LoadingId id)
        {
            if (id.Index >= 0) return id.Index < _totalController ? m_controllers[id.Index] : null;
            for (var i = m_controllers.Length - 1; i >= 0; i--)
            {
                var controller = m_controllers[i];
                if (controller.ID == id.Id) return controller;
            }

            return null;
        }

        private void LateUpdate()
        {
            for (var i = 0; i < _totalController; i++)
            {
                var typeController = m_controllers[i];
                if (typeController.IsShow == typeController.IsEnable) continue;
                typeController.gameObject.SetActive(typeController.IsShow);
            }
        }
    }
}

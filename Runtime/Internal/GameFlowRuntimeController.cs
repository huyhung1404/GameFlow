using System.Collections.Generic;
using System.Linq;
using GameFlow.Component;
using UnityEngine;

namespace GameFlow.Internal
{
    [AddComponentMenu("Game Flow/Runtime Controller")]
    internal class GameFlowRuntimeController : MonoBehaviour
    {
        private static readonly Queue<Command> s_commands = new Queue<Command>(5);
        internal static OnBannerUpdate OnBannerUpdate;
        public static bool UpdateBanner { get; set; }
        private static bool s_isLock;
        private static bool s_disableKeyBack;

        [SerializeField] private bool m_dontDestroyOnLoad = true;
        [SerializeField] private Transform m_elementContainer;
        [SerializeField] private Transform m_uiElementContainer;

        private Command _current;
        internal bool IsActive { get; private set; }

        internal static void SetLock(bool value)
        {
            s_isLock = value;
        }

        internal static void SetDisableKeyBack(bool value)
        {
            s_disableKeyBack = value;
        }

        internal static ElementCollection GetElements()
        {
            return InstanceManager.Manager.ElementCollection;
        }

        internal void SetContainer(Transform elementContainer, Transform uiElementContainer)
        {
            m_elementContainer = elementContainer;
            m_uiElementContainer = uiElementContainer;
        }

        internal static Transform PrefabElementContainer(bool isUI)
        {
            var instance = InstanceManager.Instance;
            return isUI ? instance.m_uiElementContainer : instance.m_elementContainer;
        }

        private void Awake()
        {
            if (InstanceManager.Instance != null && InstanceManager.Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Initialization();
        }

        private void Initialization()
        {
            InstanceManager.SetInstance(this);
            if (m_dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
            InstanceManager.ConfirmIsInitialized(() =>
            {
                LoadingController.Instance?.SetUpShieldSortingOrder(InstanceManager.Manager.LoadingShieldSortingOrder);
                IsActive = true;
            });
        }

        private void Update()
        {
            if (!IsActive) return;

            if (!CommandHandle())
            {
                LoadingController.EnableShield();
                return;
            }

            LoadingController.DisableShield();
            KeyBackHandle();
        }

        private void LateUpdate()
        {
            if (!UpdateBanner) return;
            UpdateBanner = false;
            OnBannerUpdate?.Invoke(FlowBannerController.CurrentBannerHeight);
        }

        internal static void AddCommand(Command command)
        {
            s_commands.Enqueue(command);
        }

        internal static void OverriderCommand(CloneCommand command)
        {
            var instance = InstanceManager.Instance;
            instance._current = command;
            instance._current.PreUpdate();
        }

        private bool CommandHandle()
        {
            if (_current != null)
            {
                _current.Update();
                if (!_current.IsRelease) return false;

                _current.OnRelease();
                _current = null;
            }

            if (s_isLock || s_commands.Count == 0) return true;

            _current = s_commands.Dequeue();
            _current.PreUpdate();
            return false;
        }

        internal void CommandsIsEmpty()
        {
            Assert.IsNotNull(InstanceManager.Instance);
            Assert.IsTrue(s_commands.Count == 0 && _current == null, CommandCountErrorMessage());
            return;

            string CommandCountErrorMessage()
            {
                var message = "Command Count: " + s_commands.Count;
                message += $"\n Current: {(_current == null ? "Empty" : _current)}";
                return s_commands.Aggregate(message, (s, c) => s + ("\n" + c));
            }
        }

        private static void KeyBackHandle()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (s_disableKeyBack || s_isLock || LoadingController.IsShow()) return;

            UIElementsRuntimeManager.OnKeyBack();
        }

        private void OnDestroy()
        {
            if (InstanceManager.Instance == this) IsActive = false;
        }

        internal static Queue<Command> GetInfo(out Command currentCommand)
        {
            currentCommand = InstanceManager.Instance?._current;
            return s_commands;
        }
    }
}
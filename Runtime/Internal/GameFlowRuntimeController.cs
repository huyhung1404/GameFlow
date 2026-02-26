using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFlow.Component;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameFlow.Internal
{
    [AddComponentMenu("Game Flow/Runtime Controller")]
    internal class GameFlowRuntimeController : MonoBehaviour
    {
        private static readonly Queue<Command> s_commands = new Queue<Command>(5);
        [SerializeField, FormerlySerializedAs("dontDestroyOnLoad")] private bool m_dontDestroyOnLoad = true;
        [SerializeField, FormerlySerializedAs("elementContainer")] private Transform m_elementContainer;
        [SerializeField, FormerlySerializedAs("uiElementContainer")] private Transform m_uiElementContainer;
        internal static OnBannerUpdate OnBannerUpdate;
        internal static bool s_UpdateBanner;
        private static GameFlowRuntimeController s_Instance;
        private static bool s_IsLock;
        private static bool s_DisableKeyBack;
        private GameFlowManager _manager;
        private Command _current;
        internal bool IsActive { get; private set; }

        internal static void SetLock(bool value)
        {
            s_IsLock = value;
        }

        internal static void SetDisableKeyBack(bool value)
        {
            s_DisableKeyBack = value;
        }

        internal static ElementCollection GetElements()
        {
            return s_Instance._manager.ElementCollection;
        }

        internal static Transform PrefabElementContainer(bool isUI)
        {
            return isUI ? s_Instance.m_uiElementContainer : s_Instance.m_elementContainer;
        }

        internal static GameFlowManager Manager() => s_Instance._manager;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (GetComponentInChildren<LoadingController>()) return;
            var loadingController = new GameObject("Loading Controller").AddComponent<LoadingController>();
            loadingController.transform.SetParent(transform);
            loadingController.GetComponent<Image>().color = Color.clear;
            var canvas = loadingController.GetComponent<Canvas>();
            canvas.sortingOrder = 100;
            canvas.planeDistance = _manager.PlaneDistance;
            UnityEditor.EditorUtility.SetDirty(gameObject);
            m_elementContainer = new GameObject("Elements").transform;
            m_elementContainer.SetParent(transform);
            m_uiElementContainer = new GameObject("UI Elements").transform;
            m_uiElementContainer.SetParent(transform);
        }
#endif

        private void Awake()
        {
            if (s_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Initialization();
        }

        private void Initialization()
        {
            s_Instance = this;
            if (m_dontDestroyOnLoad) DontDestroyOnLoad(this);
            LoadManager(3);
        }

        private void LoadManager(int timeTryGetManager)
        {
            if (timeTryGetManager <= 0) return;
            Addressables.LoadAssetAsync<GameFlowManager>(PackagePath.ManagerPath()).Completed += operationHandle =>
            {
                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    _manager = operationHandle.Result;
                    LoadingController.Instance?.SetUpShieldSortingOrder(_manager.LoadingShieldSortingOrder);
                    IsActive = true;
                    return;
                }

                ErrorHandle.LogError($"[{timeTryGetManager}] Load Game Flow Manager fail at path {PackagePath.ManagerPath()}");
                Addressables.Release(operationHandle);
                StartCoroutine(IELoadManager(timeTryGetManager - 1));
            };
        }

        private IEnumerator IELoadManager(int timeTryGetManager)
        {
            yield return new WaitForSeconds(1);
            LoadManager(timeTryGetManager);
        }

        private void Update()
        {
            if (!IsActive) return;
            if (!CommandHandle())
            {
                LoadingController.EnableTransparent();
                return;
            }

            LoadingController.DisableTransparent();
            KeyBackHandle();
        }

        private void LateUpdate()
        {
            if (!s_UpdateBanner) return;
            s_UpdateBanner = false;
            OnBannerUpdate?.Invoke(FlowBannerController.CurrentBannerHeight);
        }

        internal static void AddCommand(Command command)
        {
            s_commands.Enqueue(command);
        }

        /// <summary>
        /// Call before release old command
        /// </summary>
        /// <param name="command"></param>
        internal static void OverriderCommand(CloneCommand command)
        {
            s_Instance._current = command;
            s_Instance._current.PreUpdate();
        }

        /// <summary>
        /// Handle command
        /// </summary>
        /// <returns>True is can handle key back action</returns>
        private bool CommandHandle()
        {
            if (_current != null)
            {
                _current.Update();
                if (!_current.IsRelease) return false;
                _current.OnRelease();
                _current = null;
            }

            if (s_IsLock) return true;
            if (s_commands.Count == 0) return true;
            _current = s_commands.Dequeue();
            _current.PreUpdate();
            return false;
        }

        internal void CommandsIsEmpty()
        {
            Assert.IsNotNull(s_Instance);
            Assert.IsTrue(s_commands.Count == 0 && _current == null, CommandCountErrorMessage());
            return;

            string CommandCountErrorMessage()
            {
                var message = "Command Count: " + s_commands.Count;
                message += $"\n Current: {(_current == null ? "Empty" : _current)}";
                return s_commands.Aggregate(message, (s, c) => s + ("\n" + c));
            }
        }

        private void KeyBackHandle()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (s_DisableKeyBack) return;
            if (s_IsLock) return;
            if (LoadingController.IsShow()) return;
            UIElementsRuntimeManager.OnKeyBack();
        }

        private void OnDestroy()
        {
            if (s_Instance != null) IsActive = false;
            StopAllCoroutines();
        }

        internal static Queue<Command> GetInfo(out Command currentCommand)
        {
            currentCommand = s_Instance?._current;
            return s_commands;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFlow.Component;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace GameFlow.Internal
{
    [AddComponentMenu("Game Flow/Runtime Controller")]
    internal class GameFlowRuntimeController : MonoBehaviour
    {
        private static readonly Queue<Command> commands = new Queue<Command>(5);
        [SerializeField] private Transform elementContainer;
        [SerializeField] private Transform uiElementContainer;
        internal static OnBannerUpdate onBannerUpdate;
        internal static bool updateBanner;
        private static GameFlowRuntimeController instance;
        private static bool isLock;
        private static bool disableKeyBack;
        private GameFlowManager manager;
        private Command current;
#if UNITY_EDITOR
        internal bool isActive { get; private set; }
#else
        internal bool isActive;
#endif

        internal static void SetLock(bool value)
        {
            isLock = value;
        }

        internal static void SetDisableKeyBack(bool value)
        {
            disableKeyBack = value;
        }

        internal static ElementCollection GetElements()
        {
            return instance.manager.elementCollection;
        }

        internal static Transform PrefabElementContainer(bool isUI)
        {
            return isUI ? instance.uiElementContainer : instance.elementContainer;
        }

        internal static GameFlowManager Manager() => instance.manager;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (GetComponentInChildren<LoadingController>()) return;
            var loadingController = new GameObject("Loading Controller").AddComponent<LoadingController>();
            loadingController.transform.SetParent(transform);
            loadingController.GetComponent<Image>().color = Color.clear;
            var canvas = loadingController.GetComponent<Canvas>();
            canvas.sortingOrder = 100;
            UnityEditor.EditorUtility.SetDirty(gameObject);
            elementContainer = new GameObject("Elements").transform;
            elementContainer.SetParent(transform);
            uiElementContainer = new GameObject("UI Elements").transform;
            uiElementContainer.SetParent(transform);
        }
#endif

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Initialization();
        }

        private void Initialization()
        {
            instance = this;
            DontDestroyOnLoad(this);
            LoadManager(3);
        }

        private void LoadManager(int timeTryGetManager)
        {
            if (timeTryGetManager <= 0) return;
            Addressables.LoadAssetAsync<GameFlowManager>(PackagePath.ManagerPath()).Completed += operationHandle =>
            {
                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    manager = operationHandle.Result;
                    LoadingController.instance?.SetUpShieldSortingOrder(manager.loadingShieldSortingOrder);
                    isActive = true;
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
            if (!isActive) return;
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
            if (!updateBanner) return;
            updateBanner = false;
            onBannerUpdate?.Invoke(FlowBannerController.CurrentBannerHeight);
        }

        internal static void AddCommand(Command command)
        {
            commands.Enqueue(command);
        }

        /// <summary>
        /// Handle command
        /// </summary>
        /// <returns>True is can handle key back action</returns>
        private bool CommandHandle()
        {
            if (current != null)
            {
                current.Update();
                if (!current.isRelease) return false;
                current.OnRelease();
                current = null;
            }

            if (isLock) return true;
            if (commands.Count == 0) return true;
            current = commands.Dequeue();
            current.PreUpdate();
            return false;
        }

        internal void CommandsIsEmpty()
        {
            Assert.IsNotNull(instance);
            Assert.IsTrue(commands.Count == 0 && current == null, CommandCountErrorMessage());
            return;

            string CommandCountErrorMessage()
            {
                var message = "Command Count: " + commands.Count;
                message += $"\n Current: {(current == null ? "Empty" : current)}";
                return commands.Aggregate(message, (s, c) => s + ("\n" + c));
            }
        }

        private void KeyBackHandle()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (disableKeyBack) return;
            if (isLock) return;
            if (LoadingController.IsShow()) return;
            UIElementsRuntimeManager.OnKeyBack();
        }

        private void OnDestroy()
        {
            if (instance != null) isActive = false;
            StopAllCoroutines();
        }

        internal static Queue<Command> GetInfo(out Command currentCommand)
        {
            currentCommand = instance?.current;
            return commands;
        }
    }
}
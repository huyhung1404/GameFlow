using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameFlow.Internal
{
    internal class GameFlowRuntimeController : MonoBehaviour
    {
        private const string kGameFlowManagerPath = "Assets/GameFlow/GameFlowManager.asset";
        private static GameFlowRuntimeController instance;
#if UNITY_EDITOR
        internal static bool isActive { get; private set; }
#else
        internal static bool isActive;
#endif

#if UNITY_EDITOR
        internal GameFlowManager gameFlowManager { get; private set; }
#else
        internal GameFlowManager gameFlowManager;
#endif

        private bool isLock;

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
            Addressables.LoadAssetAsync<GameFlowManager>(kGameFlowManagerPath).Completed += operationHandle =>
            {
                if (operationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    gameFlowManager = operationHandle.Result;
                    isActive = true;
                    return;
                }

                ErrorHandle.LogError($"[{timeTryGetManager}] Load Game Flow Manager fail at path {kGameFlowManagerPath}");
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
            if (isLock) return;
            if (!CommandHandle()) return;
            KeyBackHandle();
        }

        #region Command Handle

        private readonly Queue<Command> commands = new Queue<Command>(5);
        private Command current;

        internal static void AddCommand(Command command)
        {
            if (!isActive)
            {
                ErrorHandle.LogError($"Runtime Controller is not active: [Controller: {instance}] [GameFlowManager :{instance?.gameFlowManager}]");
                return;
            }

            instance.commands.Enqueue(command);
        }

        /// <summary>
        /// Handle command
        /// </summary>
        /// <returns>True if can handle key back action</returns>
        private bool CommandHandle()
        {
            if (current != null)
            {
                if (!current.isRelease) return false;
                current = null;
            }

            if (commands.Count == 0) return true;
            current = commands.Dequeue();
            current.Execute();
            return false;
        }

        internal static void CommandsIsEmpty()
        {
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.commands.Count == 0, "commands.Count != 0");
            Assert.IsTrue(instance.current == null, instance.current?.ToString());
        }

        #endregion

        #region Key Back Handle

        private bool disableKeyBack;

        private void KeyBackHandle()
        {
            if (disableKeyBack) return;
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (LoadingController.IsShow()) return;
            //TODO: Handle Key Back
        }

        #endregion

        private void OnDestroy()
        {
            if (instance != null) isActive = false;
            StopAllCoroutines();
        }
    }
}
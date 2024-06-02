using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameFlow.Internal
{
    internal class GameFlowRuntimeController : MonoBehaviour
    {
        private static readonly Queue<Command> commands = new Queue<Command>(5);
        private static GameFlowRuntimeController instance;
        private GameFlowManager manager;
        private Command current;
        private bool isActive;
        private bool isLock;
        private bool disableKeyBack;

        internal static ElementCollection GetElements()
        {
            return instance.manager.elementCollection;
        }

        internal static Transform PrefabElementContainer()
        {
            return instance.transform;
        }

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
            if (isLock) return;
            if (!CommandHandle()) return;
            KeyBackHandle();
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

            if (commands.Count == 0) return true;
            current = commands.Dequeue();
            current.Update();
            return false;
        }

        internal static void CommandsIsEmpty()
        {
            Assert.IsNotNull(instance);
            Assert.IsTrue(commands.Count == 0, "commands.Count != 0");
            Assert.IsTrue(instance.current == null, instance.current?.ToString());
        }

        private void KeyBackHandle()
        {
            if (disableKeyBack) return;
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (LoadingController.IsShow()) return;
            //TODO: Handle Key Back
        }

        private void OnDestroy()
        {
            if (instance != null) isActive = false;
            StopAllCoroutines();
        }
    }
}
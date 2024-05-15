using System.Collections.Generic;
using UnityEngine;

namespace GameFlow.Internal
{
    internal class GameFlowRuntimeController : MonoBehaviour
    {
        internal static GameFlowRuntimeController instance;
        internal static bool isActive;

        [SerializeField] internal LoadingController loadingController;

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
            isActive = true;
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (isLock) return;
            if (!CommandHandle()) return;
            KeyBackHandle();
        }

        #region Command Handle

        private readonly Queue<Command> commands = new Queue<Command>(5);
        private Command current;

        internal void AddCommand(Command command)
        {
            commands.Enqueue(command);
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
            Assert.IsTrue(instance != null);
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
            if (loadingController.IsShow()) return;
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
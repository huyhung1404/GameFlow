using System;
using System.Collections.Generic;
using System.Linq;
using GameFlow.Component;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFlow.Internal
{
    [AddComponentMenu("Game Flow/Runtime Controller")]
    internal class GameFlowRuntimeController : MonoBehaviour
    {
        [SerializeField] private bool m_createScene = true;
        [SerializeField] private Transform m_elementContainer;
        [SerializeField] private Transform m_uiElementContainer;

        private readonly Queue<Command> _commands = new Queue<Command>(5);
        private Command _current;
        private Command _priorityCurrent;
        private GameFlowContext _context;
        private bool _isLock;
        private bool _disableKeyBack;
        private Action _onPriorityCompleted;
        internal bool NeedUpdateBanner;
        internal OnBannerUpdate OnBannerUpdateEvent;

        internal bool IsActive { get; private set; }

        internal void SetLock(bool value)
        {
            _isLock = value;
        }

        internal void SetDisableKeyBack(bool value)
        {
            _disableKeyBack = value;
        }

        internal ElementCollection GetElements()
        {
            return _context.Manager.ElementCollection;
        }

        internal void SetContainer(Transform elementContainer, Transform uiElementContainer)
        {
            m_elementContainer = elementContainer;
            m_uiElementContainer = uiElementContainer;
        }

        internal Transform PrefabElementContainer(bool isUI)
        {
            return isUI ? m_uiElementContainer : m_elementContainer;
        }

        private void Awake()
        {
            var context = GameFlowContext.Current;
            if (context?.RuntimeController != null && context.RuntimeController != this)
            {
                Destroy(gameObject);
                return;
            }

            Initialization();
        }

        private void Initialization()
        {
            if (m_createScene)
            {
                var persistentScene = SceneManager.CreateScene("[Auto] GameFlow");
                SceneManager.MoveGameObjectToScene(gameObject, persistentScene);
            }

            InstanceManager.SetInstance(this);
            InstanceManager.ConfirmIsInitialized(OnContextReady);
        }

        private void OnContextReady()
        {
            _context = GameFlowContext.Current;
            if (_context == null) return;

            if (_context.RuntimeController != null && _context.RuntimeController != this)
            {
                Destroy(gameObject);
                return;
            }

            _context.SetRuntimeController(this);
            _context.Loading?.SetUpShieldSortingOrder(_context.Manager.LoadingShieldSortingOrder);
            IsActive = true;
        }

        private void Update()
        {
            if (!IsActive) return;
#if UNITY_EDITOR
            CheckLeakedCommands();
#endif
            if (!CommandHandle())
            {
                _context.Loading?.EnableShield();
                return;
            }

            _context.Loading?.DisableShield();
            KeyBackHandle();
        }

#if UNITY_EDITOR
        private const int k_leakCheckDelayFrames = 60;

        private void CheckLeakedCommands()
        {
            var waitList = Command.s_WaitBuildCommands;
            var currentFrame = Time.frameCount;
            for (var i = waitList.Count - 1; i >= 0; i--)
            {
                var command = waitList[i];
                if (currentFrame - command.CreatedFrame < k_leakCheckDelayFrames) continue;
                ErrorHandle.LogWarning($"Command {command.GetType().Name} for '{command}' was created {currentFrame - command.CreatedFrame} frames ago but Build() was never called.");
                waitList.RemoveAt(i);
            }
        }
#endif

        private void LateUpdate()
        {
            if (!NeedUpdateBanner) return;
            NeedUpdateBanner = false;
            OnBannerUpdateEvent?.Invoke(FlowBannerController.CurrentBannerHeight);
        }

        internal void AddCommand(Command command)
        {
            _commands.Enqueue(command);
        }

        internal void SetPriorityCommand(Command command, Action onCompleted)
        {
#if UNITY_EDITOR
            Command.s_WaitBuildCommands.Remove(command);
#endif
            _priorityCurrent = command;
            _priorityCurrent.Context = _context;
            _priorityCurrent.PreUpdate();
            _onPriorityCompleted = onCompleted;
        }

        internal void OverrideCurrentCommand(CloneCommand command)
        {
            if (_current != null && !_current.IsRelease)
            {
                ErrorHandle.LogWarning($"Overriding active command '{_current}' with clone command for '{command}'.");
            }

            _current = command;
            _current.Context = _context;
            _current.PreUpdate();
        }

        private bool CommandHandle()
        {
            if (HandlePriorityCommands()) return false;

            if (_current != null)
            {
                _current.Update();
                if (!_current.IsRelease) return false;

                _current.OnRelease();
                _current = null;
            }

            if (_isLock || _commands.Count == 0) return true;

            _current = _commands.Dequeue();
            _current.Context = _context;
            _current.PreUpdate();
            return false;
        }

        private bool HandlePriorityCommands()
        {
            if (_priorityCurrent == null) return false;

            _priorityCurrent.Update();
            if (!_priorityCurrent.IsRelease) return true;
            _priorityCurrent.OnRelease();
            _priorityCurrent = null;

            var onCompleted = _onPriorityCompleted;
            _onPriorityCompleted = null;
            onCompleted?.Invoke();
            return _priorityCurrent != null;
        }

        internal void AssertCommandsEmpty()
        {
            Assert.IsTrue(_commands.Count == 0 && _current == null, CommandCountErrorMessage());
            return;

            string CommandCountErrorMessage()
            {
                var message = "Command Count: " + _commands.Count;
                message += $"\n Current: {(_current == null ? "Empty" : _current)}";
                return _commands.Aggregate(message, (s, c) => s + ("\n" + c));
            }
        }

        private void KeyBackHandle()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (_disableKeyBack || _isLock || (_context.Loading != null && _context.Loading.IsShow())) return;

            _context.UIElementsRuntime.OnKeyBack();
        }

        private void OnDestroy()
        {
            var context = GameFlowContext.Current;
            if (context != null && context.RuntimeController == this)
            {
                IsActive = false;
            }
        }

        internal Queue<Command> GetInfo(out Command currentCommand)
        {
            currentCommand = _current;
            return _commands;
        }
    }
}

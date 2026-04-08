using System;
using GameFlow.Internal;
using UnityEngine;

namespace GameFlow
{
    public abstract class Command
    {
#if UNITY_EDITOR
        internal static readonly System.Collections.Generic.List<Command> s_WaitBuildCommands = new System.Collections.Generic.List<Command>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            WarnLeakedCommands();
            s_WaitBuildCommands.Clear();
        }

        private static void WarnLeakedCommands()
        {
            foreach (var command in s_WaitBuildCommands)
            {
                ErrorHandle.LogWarning($"Command {command.GetType().Name} for '{command._elementType.Name}' was created but Build() was never called. This is likely a bug.");
            }
        }

        private readonly int _createdFrame;
        internal int CreatedFrame => _createdFrame;
#endif
        
        internal bool IsRelease { get; private set; }
        internal GameFlowContext Context { get; set; }
        protected readonly Type _elementType;

        protected Command(Type elementType)
        {
#if UNITY_EDITOR
            s_WaitBuildCommands.Add(this);
            _createdFrame = Time.frameCount;
#endif
            _elementType = elementType;
        }

        internal void Release()
        {
            IsRelease = true;
        }

        internal virtual void OnRelease()
        {
        }

        internal abstract void PreUpdate();

        internal abstract void Update();

        public void Build()
        {
#if UNITY_EDITOR
            s_WaitBuildCommands.Remove(this);
#endif
            var context = GameFlowContext.Current;
            if (context?.RuntimeController == null)
            {
                ErrorHandle.LogError($"Cannot build command '{GetType().Name}': GameFlowContext or RuntimeController is not initialized.");
                return;
            }

            context.RuntimeController.AddCommand(this);
        }

        public override string ToString()
        {
            return $"name: {_elementType.FullName} - isRelease: {IsRelease}";
        }

        internal string GetInfo()
        {
            return $"{GetType().Name}:  {_elementType.Namespace}.<b>{_elementType.Name}</b>";
        }

        internal abstract string GetFullInfo();
    }
}

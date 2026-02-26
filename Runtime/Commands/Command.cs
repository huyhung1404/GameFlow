using System;
using GameFlow.Internal;

namespace GameFlow
{
    public abstract class Command
    {
#if UNITY_EDITOR
        internal static readonly System.Collections.Generic.List<Command> s_WaitBuildCommands = new System.Collections.Generic.List<Command>();
#endif
        
        internal bool IsRelease { get; private set; }
        protected readonly Type _elementType;

        protected Command(Type elementType)
        {
#if UNITY_EDITOR
            s_WaitBuildCommands.Add(this);
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

        /// <summary>
        /// Call every frame
        /// </summary>
        internal abstract void Update();

        public void Build()
        {
#if UNITY_EDITOR
            s_WaitBuildCommands.Remove(this);
#endif
            GameFlowRuntimeController.AddCommand(this);
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
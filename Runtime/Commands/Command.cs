using System;
using GameFlow.Internal;

namespace GameFlow
{
    public abstract class Command
    {
#if UNITY_EDITOR
        internal static readonly System.Collections.Generic.List<Command> waitBuildCommands = new System.Collections.Generic.List<Command>();
#endif


#if UNITY_EDITOR
        internal bool isRelease { get; private set; }
#else
        internal bool isRelease;
#endif

        protected readonly Type elementType;

        protected Command(Type elementType)
        {
#if UNITY_EDITOR
            waitBuildCommands.Add(this);
#endif
            this.elementType = elementType;
        }

        internal void Release()
        {
            isRelease = true;
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
            waitBuildCommands.Remove(this);
#endif
            GameFlowRuntimeController.AddCommand(this);
        }

        public override string ToString()
        {
            return $"name: {elementType.FullName} - isRelease: {isRelease}";
        }

        internal string GetInfo()
        {
            return $"{GetType().Name}:  {elementType.Namespace}.<b>{elementType.Name}</b>";
        }

        public abstract string GetFullInfo();
    }
}
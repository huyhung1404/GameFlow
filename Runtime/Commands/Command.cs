using System;
using GameFlow.Internal;

namespace GameFlow
{
    public abstract class Command
    {
#if UNITY_EDITOR
        internal bool isRelease { get; private set; }
#else
        internal bool isRelease;
#endif

        protected Type elementType;

        protected Command(Type elementType)
        {
            this.elementType = elementType;
        }

        internal void Release()
        {
            isRelease = true;
        }

        internal abstract void Execute();

        public void Build()
        {
            GameFlowRuntimeController.AddCommand(this);
        }
    }
}
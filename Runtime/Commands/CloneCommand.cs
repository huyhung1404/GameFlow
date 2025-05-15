using System;
using GameFlow.Internal;

namespace GameFlow
{
    internal class CloneCommand : Command
    {
        public CloneCommand(Type elementType, Command baseCommand) : base(elementType)
        {
        }

        internal override void PreUpdate()
        {
            throw new NotImplementedException();
        }

        internal override void Update()
        {
            throw new NotImplementedException();
        }

        internal override string GetFullInfo()
        {
            throw new NotImplementedException();
        }

        internal void BuildClone()
        {
            GameFlowRuntimeController.OverriderCommand(this);
        }
    }
}
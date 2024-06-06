using System;

namespace GameFlow
{
    public class ReleaseUserInterfaceCommand : ReleaseCommand
    {
        public ReleaseUserInterfaceCommand(Type elementType, string id) : base(elementType, id)
        {
        }

        internal override void PreUpdate()
        {
            
        }
    }
}
using System;

namespace GameFlow
{
    public class AddCommand : Command
    {
        internal AddCommand(Type elementType) : base(elementType)
        {
        }

        internal override void Execute()
        {
        }
    }
}
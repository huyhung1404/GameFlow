using System;

namespace GameFlow
{
    public class AddUserInterfaceCommand : AddCommand
    {
        internal AddUserInterfaceCommand(Type elementType, string id) : base(elementType, id)
        {
        }
    }
}
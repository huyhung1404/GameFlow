using System;

namespace GameFlow
{
    public class AddCommand : Command
    {
        private bool isExecute;

        internal AddCommand(Type elementType) : base(elementType)
        {
            isExecute = false;
        }

        internal override void Update()
        {
            if (isExecute) return;
            isExecute = Execute();
        }

        private bool Execute()
        {
            return true;
        }
    }
}
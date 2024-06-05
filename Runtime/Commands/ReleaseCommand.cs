using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class ReleaseCommand : Command
    {
        private bool isExecute;
        private readonly string id;

        internal ReleaseCommand(Type elementType, string id) : base(elementType)
        {
            isExecute = false;
            this.id = id;
        }

        internal override void PreUpdate()
        {
            
        }

        internal override void Update()
        {
            if (isExecute) return;
            isExecute = Execute();
        }

        private bool Execute()
        {
            try
            {
                // if (!GetElementsIfNeed()) return true;
                // var reference = baseElement.reference;
                // if (!reference.IsDone) return false;
                // Loading();
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, $"Release Command Error: {elementType.Name}");
                OnLoadResult(false);
            }
            
            return true;
        }

        protected void OnLoadResult(bool canRelease)
        {
            Release();
        }
    }
}
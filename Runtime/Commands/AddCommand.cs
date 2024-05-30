using System;
using GameFlow.Internal;

namespace GameFlow
{
    public class AddCommand : Command
    {
        private bool isExecute;
        private readonly string id;
        private GameFlowElement element;

        internal AddCommand(Type elementType, string id) : base(elementType)
        {
            isExecute = false;
            this.id = id;
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
                if (!GetElementsIfNeed()) return true;
                var reference = element.reference;
                if (!reference.IsDone) return false;
                AddElement();
                return true;
            }
            catch (Exception e)
            {
                ErrorHandle.LogException(e, $"Add Command Error: {elementType.Name}");
                return true;
            }
        }

        private bool GetElementsIfNeed()
        {
            if (element != null) return true;
            var collection = GameFlowRuntimeController.GetElements();
            if (collection.TryGetElement(elementType, id, out element)) return true;
            ErrorHandle.LogError($"Element type {elementType.Name} not exits");
            return false;
        }

        private void AddElement()
        {
        }
    }
}
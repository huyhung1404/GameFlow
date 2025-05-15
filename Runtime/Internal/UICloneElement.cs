namespace GameFlow.Internal
{
    internal class UICloneElement : CloneElement
    {
        public UICloneElement(UIFlowElement baseElement) : base(baseElement)
        {
        }

        internal override void ActiveElement()
        {
            throw new System.NotImplementedException();
        }
    }
}
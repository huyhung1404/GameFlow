namespace GameFlow.Internal
{
    public class CloneFlowElement : CloneElement
    {
        public CloneFlowElement(GameFlowElement baseElement) : base(baseElement)
        {
        }

        internal override void ActiveElement()
        {
            throw new System.NotImplementedException();
        }
    }
}
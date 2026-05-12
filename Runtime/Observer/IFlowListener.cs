namespace GameFlow
{
    public interface IFlowListener
    {
        public void OnActive();
        public void OnRelease(bool isReleaseImmediately);
        public void OnKeyBack();
        public void OnReFocus();
    }
}
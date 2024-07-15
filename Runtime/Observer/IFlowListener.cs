namespace GameFlow
{
    public interface IFlowListener
    {
        public void OnActive();
        public void OnActiveWithData(object data);
        public void OnRelease(bool isReleaseImmediately);
        public void OnKeyBack();
        public void OnReFocus();
    }
}
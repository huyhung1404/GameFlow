namespace GameFlow
{
    public interface ICommandReleaseHandle
    {
        public void Next();
    }

    internal interface IReleaseCompleted
    {
        internal void UnloadCompleted(bool isSuccess);
    }
}
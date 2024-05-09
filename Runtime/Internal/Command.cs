namespace GameFlow.Internal
{
    internal abstract class Command
    {
        private bool isRelease;

        internal bool IsRelease() => isRelease;

        internal void Release()
        {
            isRelease = false;
        }

        internal abstract void Execute();
    }
}
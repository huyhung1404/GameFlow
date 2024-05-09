namespace GameFlow.Internal
{
    internal abstract class Command
    {
        private bool isRelease;

        internal bool IsRelease() => isRelease;

        internal void Release()
        {
            isRelease = true;
        }

        internal abstract void Execute();
    }
}
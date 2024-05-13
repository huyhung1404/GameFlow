namespace GameFlow.Internal
{
    internal abstract class Command
    {
#if UNITY_EDITOR
        internal bool isRelease { get; private set; }
#else
        internal bool isRelease;
#endif

        internal void Release()
        {
            isRelease = true;
        }

        internal abstract void Execute();
    }
}
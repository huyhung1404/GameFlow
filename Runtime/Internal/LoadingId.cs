namespace GameFlow.Internal
{
    internal struct LoadingId
    {
        public readonly string Id;
        public readonly int Index;

        internal LoadingId(string id)
        {
            Id = id;
            Index = -1;
        }

        internal LoadingId(int index)
        {
            Id = null;
            Index = index;
        }
    }
}
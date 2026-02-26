using System;

namespace GameFlow
{
    [Serializable]
    public enum ElementReleaseMode
    {
        ReleaseOnClose,
        ReleaseOnCloseIncludeCallback,
        NoneRelease
    }
}
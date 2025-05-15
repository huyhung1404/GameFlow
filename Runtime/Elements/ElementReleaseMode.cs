using System;

namespace GameFlow
{
    [Serializable]
    public enum ElementReleaseMode
    {
        RELEASE_ON_CLOSE,
        RELEASE_ON_CLOSE_INCLUDE_CALLBACK,
        NONE_RELEASE
    }
}
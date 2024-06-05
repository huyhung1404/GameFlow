namespace GameFlow.Internal
{
    internal static class Utility
    {
        internal static bool FlowIDEquals(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2)) return true;
            return string.Equals(s1, s2);
        }
    }
}
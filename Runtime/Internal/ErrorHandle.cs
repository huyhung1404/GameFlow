using UnityEngine;

namespace GameFlow.Internal
{
    public static class ErrorHandle
    {
        private const string LOG_FORMAT = "GameFlow: {0}";

        public static void LogError(string content)
        {
            Debug.LogErrorFormat(LOG_FORMAT, content);
        }
    }
}
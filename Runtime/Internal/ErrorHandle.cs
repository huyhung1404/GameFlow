using System;
using UnityEngine;

namespace GameFlow.Internal
{
    internal static class ErrorHandle
    {
        private const string LOG_FORMAT = "GameFlow: {0}";

        public static void LogError(string content)
        {
            Debug.LogErrorFormat(LOG_FORMAT, content);
        }

        public static void LogWarning(string content)
        {
            Debug.LogWarningFormat(LOG_FORMAT, content);
        }

        public static void LogException(Exception e, string content)
        {
            Debug.LogError(content);
            Debug.LogException(e);
        }
    }
}
using System;
using UnityEngine;

namespace GameFlow.Internal
{
    internal static class ErrorHandle
    {
        private const string k_LogFormat = "GameFlow: {0}";

        public static void LogError(string content)
        {
            Debug.LogErrorFormat(k_LogFormat, content);
        }

        public static void LogWarning(string content)
        {
            Debug.LogWarningFormat(k_LogFormat, content);
        }

        public static void LogException(Exception e, string content)
        {
            Debug.LogError(content);
            Debug.LogException(e);
        }
    }
}
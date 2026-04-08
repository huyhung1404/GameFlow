using System;
using UnityEngine;

namespace GameFlow.Internal
{
    public enum FlowLogLevel
    {
        Warning,
        Error,
        Exception
    }

    public static class ErrorHandle
    {
        private const string k_logFormat = "GameFlow: {0}";

        public static event Action<FlowLogLevel, string, Exception> OnLog;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            OnLog = null;
        }
#endif

        internal static void LogError(string content)
        {
            Debug.LogErrorFormat(k_logFormat, content);
            OnLog?.Invoke(FlowLogLevel.Error, content, null);
        }

        internal static void LogWarning(string content)
        {
            Debug.LogWarningFormat(k_logFormat, content);
            OnLog?.Invoke(FlowLogLevel.Warning, content, null);
        }

        internal static void LogException(Exception e, string content)
        {
            Debug.LogError(content);
            Debug.LogException(e);
            OnLog?.Invoke(FlowLogLevel.Exception, content, e);
        }
    }
}
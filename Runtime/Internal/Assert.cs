using UnityEngine;

namespace GameFlow.Internal
{
    public static class Assert
    {
        public static void IsTrue(bool condition, string message = null)
        {
            UnityEngine.Assertions.Assert.IsTrue(condition, message);
        }

        public static void AreEqual<T>(T expected, T actual, string message = null)
        {
            UnityEngine.Assertions.Assert.AreEqual(expected, actual, message);
        }

        public static void IsGameObjectEnable(MonoBehaviour o, string message = null)
        {
            UnityEngine.Assertions.Assert.IsTrue(o.gameObject.activeSelf, message);
        }

        public static void IsGameObjectDisable(MonoBehaviour o, string message = null)
        {
            UnityEngine.Assertions.Assert.IsTrue(!o.gameObject.activeSelf, message);
        }

        public static void IsNotNull<T>(T value, string message = null) where T : Object
        {
            UnityEngine.Assertions.Assert.IsNotNull(value, message);
        }

        public static void IsValidReference<T>(bool condition, string id = null) where T : GameFlowElement
        {
            IsTrue(GameFlowRuntimeController.GetElements().GetElement(typeof(T), id).runtimeInstance == condition);
        }
    }
}
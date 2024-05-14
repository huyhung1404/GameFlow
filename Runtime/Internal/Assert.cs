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
    }
}
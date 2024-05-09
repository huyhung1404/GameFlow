namespace GameFlow.Internal
{
    public static class Assert
    {
        public static void IsTrue(bool condition, string message = null)
        {
            UnityEngine.Assertions.Assert.IsTrue(condition, message);
        }
    }
}
using System.Collections;
using GameFlow.Tests.Build;
using UnityEngine.TestTools;

namespace GameFlow.Tests
{
    public class ResourcesTests : BaseTests
    {
        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            yield return ResourcesInstance.Load();
        }

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            yield return ResourcesInstance.Unload();
        }
    }
}
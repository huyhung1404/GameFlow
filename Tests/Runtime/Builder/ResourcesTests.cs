using System.Collections;
using UnityEngine.TestTools;

namespace GameFlow.Tests.Build
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
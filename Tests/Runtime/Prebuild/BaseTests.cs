using System.Collections;
using GameFlow.Tests.Build;
using UnityEngine.TestTools;

namespace GameFlow.Tests
{
    public class BaseTests : IPrebuildSetup, IPostBuildCleanup
    {
        public void Setup()
        {
            Prebuild.PresetResources();
        }

        public void Cleanup()
        {
            Prebuild.CleanupResources();
        }

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
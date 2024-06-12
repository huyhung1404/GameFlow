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
    }
}
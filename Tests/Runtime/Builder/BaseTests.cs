using UnityEngine.TestTools;

namespace GameFlow.Tests.Build
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
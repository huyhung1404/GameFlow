using GameFlow.Tests.Build;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameFlow.Tests
{
    public class CommandTests : IPrebuildSetup, IPostBuildCleanup
    {
        public void Setup()
        {
            Prebuild.PresetResources();
        }

        [Test]
        public void WhenNullTextureIsPassed_CreateShouldReturnNullSprite()
        {
        }

        public void Cleanup()
        {
            Prebuild.CleanupResources();
        }
    }
}
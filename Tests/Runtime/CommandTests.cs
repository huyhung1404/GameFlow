using GameFlow.Tests.Build;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameFlow.Tests
{
    public class CommandTests : IPrebuildSetup
    {
        public void Setup()
        {
            PrebuildSetup.CheckingResource();
        }

        [Test]
        public void WhenNullTextureIsPassed_CreateShouldReturnNullSprite()
        {
        }
    }
}
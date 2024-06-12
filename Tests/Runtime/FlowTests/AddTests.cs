using System.Collections;
using GameFlow.Tests.Build;
using UnityEngine.TestTools;

namespace GameFlow.Tests
{
    public class AddTests : ResourcesTests
    {
        [UnitySetUp]
        public new IEnumerator UnitySetUp()
        {
            yield return null;
        }

        [UnityTest]
        public IEnumerator Add_Active_SingleElement()
        {
            yield return null;
        }
    }
}
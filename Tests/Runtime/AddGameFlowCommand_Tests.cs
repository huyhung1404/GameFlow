using System.Collections;
using System.Collections.Generic;
using GameFlow.Internal;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = GameFlow.Internal.Assert;

namespace GameFlow.Tests
{
    public class AddGameFlowCommand_Tests
    {
        public class TestScript___ElementAddPrefab : GameFlowElement
        {
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            var controller = Builder.CreateMono<GameFlowRuntimeController>();
            controller.CreateChildMono<LoadingController>();
            yield return null;
        }

        [UnityTest]
        public IEnumerator Single_Add_Execute_Command()
        {
            yield return null;
        }
    }
}
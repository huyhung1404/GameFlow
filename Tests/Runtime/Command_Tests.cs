using System.Collections;
using GameFlow.Internal;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = GameFlow.Internal.Assert;

namespace GameFlow.Tests
{
    public class Command_Tests
    {
        private class EmptyCommand : Command
        {
            internal bool isExecute;

            internal override void Execute()
            {
                isExecute = true;
            }
        }

        [UnityTest]
        public IEnumerator Simple_Add_Command()
        {
            InitController();
            yield return null;
            var command = new EmptyCommand();
            FlowController.instance.AddCommand(command);
            yield return null;
            yield return null;
            yield return null;
            command.Release();
            yield return null;
            yield return null;
            Assert.IsTrue(command.isExecute);
            FlowController.instance.CommandsIsEmpty();
        }

        private static void InitController()
        {
            var o = Object.Instantiate(new GameObject()).AddComponent<FlowController>();
            o.loadingController = Object.Instantiate(new GameObject()).AddComponent<LoadingController>();
        }
    }
}
using System.Collections;
using GameFlow.Tests.Build;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = GameFlow.Internal.Assert;

namespace GameFlow.Tests
{
    public class AddTests : ResourcesTests
    {
        [SetUp]
        public void SetUp()
        {
            CallbackHistory.current = new CallbackHistory();
        }

        [UnityTest]
        public IEnumerator Add_Active_SingleElement()
        {
            var next = false;
            GameCommand.Add<TestScript___SimpleElement>().OnCompleted(_ =>
            {
                ResourcesInstance.loadingController.IsTransparentOn();
                next = true;
            }).Build();
            while (!next) yield return null;
            ResourcesInstance.runtimeController.CommandsIsEmpty();
            Debug.Log(CallbackHistory.current.ToString());
            CallbackHistory.HistoryCountEquals(1);
            CallbackHistory.RecordCountEquals(1);
            Assert.IsTrue(CallbackHistory.GetRecord(0).gameObject.activeSelf);
            CallbackHistory.CheckHistoryIndex(0, typeof(CallbackHistory.OnActiveRecord), typeof(TestScript___SimpleElement));
        }
    }
}
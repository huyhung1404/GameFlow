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
            LogAssert.ignoreFailingMessages = false;
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

        [UnityTest]
        public IEnumerator Add_Active_SingleSceneElement()
        {
            var next = false;
            GameCommand.Add<TestScript___SimpleSceneElement>().OnCompleted(_ =>
            {
                ResourcesInstance.loadingController.IsTransparentOn();
                next = true;
            }).Build();
            while (!next) yield return null;
            yield return null;
            ResourcesInstance.runtimeController.CommandsIsEmpty();
            Debug.Log(CallbackHistory.current.ToString());
            CallbackHistory.HistoryCountEquals(1);
            CallbackHistory.RecordCountEquals(1);
            Assert.IsTrue(CallbackHistory.GetRecord(0).gameObject.activeSelf);
            CallbackHistory.CheckHistoryIndex(0, typeof(CallbackHistory.OnActiveRecord), typeof(TestScript___SimpleSceneElement));
        }

        public class TestScript___NoReference : GameFlowElement
        {
        }

        [UnityTest]
        public IEnumerator Add_ThrowError_NoReference()
        {
            LogAssert.ignoreFailingMessages = true;
            var next = false;
            GameCommand.Add<TestScript___NoReference>().OnCompleted(_ => next = true).Build();
            while (!next) yield return null;
            yield return null;
            ResourcesInstance.runtimeController.CommandsIsEmpty();
            ResourcesInstance.loadingController.IsTransparentOff();
        }

        [UnityTest]
        public IEnumerator Add_Active_DuplicateSimpleElement()
        {
            var next = false;
            GameCommand.Add<TestScript___SimpleElement>().Build();
            GameCommand.Add<TestScript___SimpleElement>().OnCompleted(_ => { next = true; }).Build();
            while (!next) yield return null;
            yield return null;
            ResourcesInstance.runtimeController.CommandsIsEmpty();
            Debug.Log(CallbackHistory.current.ToString());
            CallbackHistory.TotalExecuteHistory<CallbackHistory.OnActiveRecord>(2);
            CallbackHistory.TotalExecuteHistory<CallbackHistory.OnReleaseRecord>(1);
            Assert.IsTrue(CallbackHistory.GetRecord(0).gameObject.activeSelf);
        }
    }
}
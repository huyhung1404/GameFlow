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
            CallbackHistory.Current = new CallbackHistory();
        }

        [UnityTest]
        public IEnumerator Add_Active_SingleElement()
        {
            var next = false;
            AddCommandBuilder.OnCompleted(GameCommand.Add<TestScript___SimpleElement>(), _ =>
            {
                ResourcesInstance.LoadingController.IsTransparentOn();
                next = true;
            }).Build();
            while (!next) yield return null;
            ResourcesInstance.RuntimeController.CommandsIsEmpty();
            Debug.Log(CallbackHistory.Current.ToString());
            CallbackHistory.HistoryCountEquals(1);
            CallbackHistory.RecordCountEquals(1);
            Assert.IsTrue(CallbackHistory.GetRecord(0).gameObject.activeSelf);
            CallbackHistory.CheckHistoryIndex(0, typeof(CallbackHistory.OnActiveRecord), typeof(TestScript___SimpleElement));
        }

        [UnityTest]
        public IEnumerator Add_Active_SingleSceneElement()
        {
            var next = false;
            AddCommandBuilder.OnCompleted(GameCommand.Add<TestScript___SimpleSceneElement>(), _ =>
            {
                ResourcesInstance.LoadingController.IsTransparentOn();
                next = true;
            }).Build();
            while (!next) yield return null;
            yield return null;
            ResourcesInstance.RuntimeController.CommandsIsEmpty();
            Debug.Log(CallbackHistory.Current.ToString());
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
            AddCommandBuilder.OnCompleted(GameCommand.Add<TestScript___NoReference>(), _ => next = true).Build();
            while (!next) yield return null;
            yield return null;
            ResourcesInstance.RuntimeController.CommandsIsEmpty();
            ResourcesInstance.LoadingController.IsTransparentOff();
        }

        [UnityTest]
        public IEnumerator Add_Active_DuplicateSimpleElement()
        {
            var next = false;
            GameCommand.Add<TestScript___SimpleElement>().Build();
            AddCommandBuilder.OnCompleted(GameCommand.Add<TestScript___SimpleElement>(), _ => { next = true; }).Build();
            while (!next) yield return null;
            yield return null;
            ResourcesInstance.RuntimeController.CommandsIsEmpty();
            Debug.Log(CallbackHistory.Current.ToString());
            CallbackHistory.TotalExecuteHistory<CallbackHistory.OnActiveRecord>(2);
            CallbackHistory.TotalExecuteHistory<CallbackHistory.OnReleaseRecord>(1);
            Assert.IsTrue(CallbackHistory.GetRecord(0).gameObject.activeSelf);
        }
    }
}
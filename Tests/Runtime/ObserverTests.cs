using GameFlow.Internal;
using NUnit.Framework;
using UnityEngine;
using Assert = NUnit.Framework.Assert;

namespace GameFlow.Tests
{
    public class ObserverTests
    {
        private class TestScript___EventCallback1 : GameFlowElement
        {
            public static int timeRun;
        }

        private class TestScript___EventCallback2 : UIFlowElement
        {
            public static int timeRun;
        }

        private static void Execute1() => TestScript___EventCallback1.timeRun++;
        private static void Execute2() => TestScript___EventCallback2.timeRun++;

        [SetUp]
        public void SetUp()
        {
            var manager = ScriptableObject.CreateInstance<GameFlowManager>();
            var context = new GameFlowContext(manager);
            GameFlowContext.SetCurrent(context);
            TestScript___EventCallback1.timeRun = 0;
            TestScript___EventCallback2.timeRun = 0;
        }

        [TearDown]
        public void TearDown()
        {
            GameFlowContext.SetCurrent(null);
        }

        [Test]
        public void Execute_Simple_OnActive()
        {
            FlowObservable.Event<TestScript___EventCallback1>().OnActive += Execute1;
            FlowObservable.Event<TestScript___EventCallback1>().RaiseOnActive();
            FlowObservable.Event<TestScript___EventCallback1>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 2);
            FlowObservable.Event<TestScript___EventCallback1>().OnActive -= Execute1;
            FlowObservable.Event<TestScript___EventCallback1>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 2);
        }

        [Test]
        public void Execute_Double_OnActive()
        {
            FlowObservable.Event<TestScript___EventCallback1>().OnActive += Execute1;
            FlowObservable.Event<TestScript___EventCallback1>().RaiseOnActive();
            FlowObservable.Event<TestScript___EventCallback2>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 1);
            Assert.IsTrue(TestScript___EventCallback2.timeRun == 0);
            FlowObservable.Event<TestScript___EventCallback2>().OnActive += Execute2;
            FlowObservable.Event<TestScript___EventCallback1>().OnActive -= Execute1;
            FlowObservable.Event<TestScript___EventCallback1>().RaiseOnActive();
            FlowObservable.Event<TestScript___EventCallback2>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 1);
            Assert.IsTrue(TestScript___EventCallback2.timeRun == 1);
        }

        [Test]
        public void Execute_Simple_OnActiveDuplicateAction()
        {
            FlowObservable.Event<TestScript___EventCallback1>().OnActive += Execute1;
            FlowObservable.Event<TestScript___EventCallback1>().OnActive += Execute2;
            FlowObservable.Event<TestScript___EventCallback1>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 1);
            Assert.IsTrue(TestScript___EventCallback2.timeRun == 1);
        }
    }
}

using NUnit.Framework;

namespace GameFlow.Tests
{
    public class ObserverTests
    {
        private class TestScript___EventCallback1 : GameFlowElement
        {
            public static int timeRun;
        }

        private class TestScript___EventCallback2 : UserInterfaceFlowElement
        {
            public static int timeRun;
        }

        private static void Execute1() => TestScript___EventCallback1.timeRun++;
        private static void Execute2() => TestScript___EventCallback2.timeRun++;

        [SetUp]
        public void SetUp()
        {
            FlowSubject.callbackEvents.Clear();
            TestScript___EventCallback1.timeRun = 0;
            TestScript___EventCallback2.timeRun = 0;
        }

        [Test]
        public void Execute_Simple_OnActive()
        {
            FlowSubject.Event<TestScript___EventCallback1>().OnActive += Execute1;
            FlowSubject.Event<TestScript___EventCallback1>().RaiseOnActive();
            FlowSubject.Event<TestScript___EventCallback1>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 2);
            FlowSubject.Event<TestScript___EventCallback1>().OnActive -= Execute1;
            FlowSubject.Event<TestScript___EventCallback1>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 2);
        }

        [Test]
        public void Execute_Double_OnActive()
        {
            FlowSubject.Event<TestScript___EventCallback1>().OnActive += Execute1;
            FlowSubject.Event<TestScript___EventCallback1>().RaiseOnActive();
            FlowSubject.Event<TestScript___EventCallback2>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 1);
            Assert.IsTrue(TestScript___EventCallback2.timeRun == 0);
            FlowSubject.Event<TestScript___EventCallback2>().OnActive += Execute2;
            FlowSubject.Event<TestScript___EventCallback1>().OnActive -= Execute1;
            FlowSubject.Event<TestScript___EventCallback1>().RaiseOnActive();
            FlowSubject.Event<TestScript___EventCallback2>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 1);
            Assert.IsTrue(TestScript___EventCallback2.timeRun == 1);
        }

        [Test]
        public void Execute_Simple_OnActiveDuplicateAction()
        {
            FlowSubject.Event<TestScript___EventCallback1>().OnActive += Execute1;
            FlowSubject.Event<TestScript___EventCallback1>().OnActive += Execute2;
            FlowSubject.Event<TestScript___EventCallback1>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 1);
            Assert.IsTrue(TestScript___EventCallback2.timeRun == 1);
        }
    }
}
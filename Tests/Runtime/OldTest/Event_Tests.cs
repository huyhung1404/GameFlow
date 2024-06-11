using NUnit.Framework;
using Assert = GameFlow.Internal.Assert;

namespace GameFlow.Tests
{
    public class Event_Tests
    {
        public class TestScript___EventCallback1 : GameFlowElement
        {
            public static int timeRun;
        }

        public class TestScript___EventCallback2 : GameFlowElement
        {
            public static int timeRun;
        }

        [SetUp]
        public void SetUp()
        {
            FlowSubject.callbackEvents.Clear();
            TestScript___EventCallback1.timeRun = 0;
            TestScript___EventCallback2.timeRun = 0;
        }

        [Test]
        public void _0_Simple_Callback()
        {
            FlowSubject.Event<TestScript___EventCallback1>().OnActive += Test1;
            FlowSubject.Event<TestScript___EventCallback1>().RaiseOnActive();
            FlowSubject.Event<TestScript___EventCallback1>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 2);
            FlowSubject.Event<TestScript___EventCallback1>().OnActive -= Test1;
            FlowSubject.Event<TestScript___EventCallback1>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 2);
        }

        [Test]
        public void _1_Simple_Callback()
        {
            FlowSubject.Event<TestScript___EventCallback1>().OnActive += Test1;
            FlowSubject.Event<TestScript___EventCallback1>().RaiseOnActive();
            FlowSubject.Event<TestScript___EventCallback2>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 1);
            Assert.IsTrue(TestScript___EventCallback2.timeRun == 0);
            FlowSubject.Event<TestScript___EventCallback2>().OnActive += Test2;
            FlowSubject.Event<TestScript___EventCallback1>().OnActive -= Test1;
            FlowSubject.Event<TestScript___EventCallback1>().RaiseOnActive();
            FlowSubject.Event<TestScript___EventCallback2>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 1);
            Assert.IsTrue(TestScript___EventCallback2.timeRun == 1);
        }

        [Test]
        public void _2_Duplicate_2_Action()
        {
            FlowSubject.Event<TestScript___EventCallback1>().OnActive += Test1;
            FlowSubject.Event<TestScript___EventCallback1>().OnActive += Test2;
            FlowSubject.Event<TestScript___EventCallback1>().RaiseOnActive();
            Assert.IsTrue(TestScript___EventCallback1.timeRun == 1);
            Assert.IsTrue(TestScript___EventCallback2.timeRun == 1);
        }

        private static void Test1()
        {
            TestScript___EventCallback1.timeRun++;
        }

        private static void Test2()
        {
            TestScript___EventCallback2.timeRun++;
        }
    }
}
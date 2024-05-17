using NUnit.Framework;
using Assert = GameFlow.Internal.Assert;

namespace GameFlow.Tests
{
    public class Event_Tests
    {
        public class EventTest1 : GameFlowElement
        {
            public static int timeRun;
        }

        public class EventTest2 : GameFlowElement
        {
            public static int timeRun;
        }

        [SetUp]
        public void SetUp()
        {
            EventTest1.timeRun = 0;
            EventTest2.timeRun = 0;
        }

        [Test]
        public void _0_Simple_Callback()
        {
            GameFlowEvent.Listen<EventTest1>(Test1);
            GameFlowEvent.OnActive(typeof(EventTest1), null);
            GameFlowEvent.OnActive(typeof(EventTest1), null);
            Assert.IsTrue(EventTest1.timeRun == 2);
            GameFlowEvent.RemoveListener<EventTest1>(Test1);
            GameFlowEvent.OnActive(typeof(EventTest1), null);
            Assert.IsTrue(EventTest1.timeRun == 2);
        }

        [Test]
        public void _1_Simple_Callback()
        {
            GameFlowEvent.Listen<EventTest1>(Test1);
            GameFlowEvent.OnActive(typeof(EventTest1), null);
            GameFlowEvent.OnActive(typeof(EventTest2), null);
            Assert.IsTrue(EventTest1.timeRun == 1);
            Assert.IsTrue(EventTest2.timeRun == 0);
            GameFlowEvent.Listen<EventTest2>(Test2);
            GameFlowEvent.RemoveListener<EventTest1>(Test1);
            GameFlowEvent.OnActive(typeof(EventTest1), null);
            GameFlowEvent.OnActive(typeof(EventTest2), null);
            Assert.IsTrue(EventTest1.timeRun == 1);
            Assert.IsTrue(EventTest2.timeRun == 1);
        }

        [Test]
        public void _2_Duplicate_2_Action()
        {
            GameFlowEvent.Listen<EventTest1>(Test1);
            GameFlowEvent.Listen<EventTest1>(Test2);
            GameFlowEvent.OnActive(typeof(EventTest1), null);
            Assert.IsTrue(EventTest1.timeRun == 1);
            Assert.IsTrue(EventTest2.timeRun == 1);
        }

        private void Test1(object data)
        {
            EventTest1.timeRun++;
        }


        private void Test2(object data)
        {
            EventTest2.timeRun++;
        }
    }
}
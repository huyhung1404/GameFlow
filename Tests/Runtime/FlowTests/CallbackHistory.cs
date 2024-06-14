using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFlow.Internal;
using UnityEngine;

namespace GameFlow.Tests
{
    public class CallbackHistory
    {
        public static CallbackHistory current;
        public readonly List<RecordCallback> executeHistory;
        public readonly List<RecordObject> recorderObject;

        public class RecordObject
        {
            public GameObject gameObject;
            public string content;

            public override string ToString()
            {
                return $"{content} - Active: {gameObject?.activeSelf}";
            }
        }

        public abstract class RecordCallback
        {
            public Type type;
            public float time;

            protected RecordCallback(Type type)
            {
                this.type = type;
            }
        }

        public class OnActiveRecord : RecordCallback
        {
            public OnActiveRecord(Type type) : base(type)
            {
            }

            public override string ToString()
            {
                return $"OnActive | Type: {type.Name} Time: {time}";
            }
        }

        public class OnActiveWithDataRecord : RecordCallback
        {
            public object data;

            public OnActiveWithDataRecord(Type type, object data) : base(type)
            {
                this.data = data;
            }

            public override string ToString()
            {
                return $"OnActiveWithData | Type: {type.Name} Time: {time} Data:{data}";
            }
        }

        public class OnReleaseRecord : RecordCallback
        {
            public bool isReleaseImmediately;

            public OnReleaseRecord(Type type, bool isReleaseImmediately) : base(type)
            {
                this.isReleaseImmediately = isReleaseImmediately;
            }

            public override string ToString()
            {
                return $"OnRelease | Type: {type.Name} Time: {time} IsReleaseImmediately:{isReleaseImmediately}";
            }
        }

        public CallbackHistory()
        {
            executeHistory = new List<RecordCallback>();
            recorderObject = new List<RecordObject>();
        }

        public void RecorderObject(GameObject o, GameFlowElement element)
        {
            recorderObject.Add(new RecordObject
            {
                gameObject = o,
                content = $"GameObject: {o.name}, Element: {element.name} | "
            });
        }

        public void WriteOnActive(Type type)
        {
            executeHistory.Add(new OnActiveRecord(type)
            {
                time = Time.time
            });
        }

        public void WriteOnActiveWithData(Type type, object data)
        {
            executeHistory.Add(new OnActiveWithDataRecord(type, data)
            {
                time = Time.time
            });
        }

        public void WriteOnRelease(Type type, bool isReleaseImmediately)
        {
            executeHistory.Add(new OnReleaseRecord(type, isReleaseImmediately)
            {
                time = Time.time
            });
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendLine($"Recorder Count: {recorderObject.Count}");
            foreach (var record in recorderObject)
            {
                result.AppendLine(record.ToString());
            }

            result.AppendLine($"History Count: {executeHistory.Count}");
            foreach (var recordCallback in executeHistory)
            {
                result.AppendLine(recordCallback.ToString());
            }

            return result.ToString();
        }

        public static void HistoryCountEquals(int count)
        {
            Assert.IsTrue(current.executeHistory.Count == count);
        }

        public static void RecordCountEquals(int count)
        {
            Assert.IsTrue(current.recorderObject.Count == count);
        }

        public static void TotalExecuteHistory<T>(int count) where T : RecordCallback
        {
            Assert.IsTrue(current.executeHistory.Count(item => item is T) == count);
        }

        public static void CheckHistoryIndex(int index, Type typeRecord, Type typeElement)
        {
            Assert.IsTrue(current.executeHistory[index].GetType() == typeRecord);
            Assert.IsTrue(current.executeHistory[index].type == typeElement);
        }

        public static RecordObject GetRecord(int index)
        {
            return current.recorderObject[index];
        }
    }
}
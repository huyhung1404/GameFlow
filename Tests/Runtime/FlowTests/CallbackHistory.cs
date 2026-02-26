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
        public static CallbackHistory Current;
        public readonly List<RecordCallback> ExecuteHistory;
        public readonly List<RecordObject> RecorderObjects;

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
            ExecuteHistory = new List<RecordCallback>();
            RecorderObjects = new List<RecordObject>();
        }

        public void RecorderObject(GameObject o, GameFlowElement element)
        {
            RecorderObjects.Add(new RecordObject
            {
                gameObject = o,
                content = $"GameObject: {o.name}, Element: {element.name} | "
            });
        }

        public void WriteOnActive(Type type)
        {
            ExecuteHistory.Add(new OnActiveRecord(type)
            {
                time = Time.time
            });
        }

        public void WriteOnActiveWithData(Type type, object data)
        {
            ExecuteHistory.Add(new OnActiveWithDataRecord(type, data)
            {
                time = Time.time
            });
        }

        public void WriteOnRelease(Type type, bool isReleaseImmediately)
        {
            ExecuteHistory.Add(new OnReleaseRecord(type, isReleaseImmediately)
            {
                time = Time.time
            });
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendLine($"Recorder Count: {RecorderObjects.Count}");
            foreach (var record in RecorderObjects)
            {
                result.AppendLine(record.ToString());
            }

            result.AppendLine($"History Count: {ExecuteHistory.Count}");
            foreach (var recordCallback in ExecuteHistory)
            {
                result.AppendLine(recordCallback.ToString());
            }

            return result.ToString();
        }

        public static void HistoryCountEquals(int count)
        {
            Assert.IsTrue(Current.ExecuteHistory.Count == count);
        }

        public static void RecordCountEquals(int count)
        {
            Assert.IsTrue(Current.RecorderObjects.Count == count);
        }

        public static void TotalExecuteHistory<T>(int count) where T : RecordCallback
        {
            Assert.IsTrue(Current.ExecuteHistory.Count(item => item is T) == count);
        }

        public static void CheckHistoryIndex(int index, Type typeRecord, Type typeElement)
        {
            Assert.IsTrue(Current.ExecuteHistory[index].GetType() == typeRecord);
            Assert.IsTrue(Current.ExecuteHistory[index].type == typeElement);
        }

        public static RecordObject GetRecord(int index)
        {
            return Current.RecorderObjects[index];
        }
    }
}
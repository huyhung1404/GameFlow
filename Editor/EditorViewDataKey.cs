using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
    public static class EditorViewDataKey
    {
        private const string kDataKeyPrefs = "com.huyhung1404.gameflow_ViewDataKey";
        private static readonly Dictionary<string, bool> dataKey;

        static EditorViewDataKey()
        {
            dataKey = new Dictionary<string, bool>();
        }

        public static void OnEnable()
        {
            var jsonData = EditorPrefs.GetString(kDataKeyPrefs, null);
            if (jsonData == null)
            {
                dataKey.Clear();
                return;
            }

            try
            {
                ToData(jsonData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                dataKey.Clear();
            }
        }

        public static void OnDisable()
        {
            EditorPrefs.SetString(kDataKeyPrefs, ToStringData());
        }

        private static string ToStringData()
        {
            var result = new StringBuilder();
            foreach (var data in dataKey)
            {
                result.AppendLine(data.Key.Trim());
                result.AppendLine(data.Value.ToString());
            }

            return result.ToString();
        }

        private static void ToData(string text)
        {
            dataKey.Clear();
            var data = text.Split("\n");
            for (var i = 0; i < data.Length / 2; i++)
            {
                var key = data[i * 2].Trim();
                var value = data[i * 2 + 1].Trim();
                dataKey.Add(key, bool.Parse(value));
            }
        }

        public static void BindToViewDataKey(this Foldout foldout, string key, bool defaultValue = true)
        {
            if (!dataKey.ContainsKey(key))
            {
                dataKey.Add(key, defaultValue);
            }

            foldout.value = dataKey[key];
            foldout.UnregisterCallback<ClickEvent>(Callback);
            foldout.RegisterCallback<ClickEvent>(Callback);

            void Callback(ClickEvent e)
            {
                dataKey[key] = foldout.value;
            }
        }
    }
}
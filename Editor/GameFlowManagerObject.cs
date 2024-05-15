using System.IO;
using GameFlow.Internal;
using UnityEditor;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

namespace GameFlow.Editor
{
    public static class GameFlowManagerObject
    {
        private static GameFlowManager _instance;
        public const string kDefaultConfigAssetName = "GameFlowManager";
        public const string kDefaultConfigFolder = "Assets/GameFlow";
        public const string kPath = kDefaultConfigFolder + "/" + kDefaultConfigAssetName + ".asset";

        private const string kTemplateAsmdef = @"{
                    ""name"": ""GameFlowElements"",
                    ""rootNamespace"": ""GameFlow"",
                    ""references"": [
                    ""com.huyhung1404.gameflow""
                        ],
                    ""includePlatforms"": [],
                    ""excludePlatforms"": [],
                    ""allowUnsafeCode"": false,
                    ""overrideReferences"": false,
                    ""precompiledReferences"": [],
                    ""autoReferenced"": true,
                    ""defineConstraints"": [],
                    ""versionDefines"": [],
                    ""noEngineReferences"": false
                }";

        internal static GameFlowManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = AssetDatabase.LoadAssetAtPath<GameFlowManager>(kPath);
                return _instance;
            }
        }

        internal static void CreateDefaultInstance()
        {
            var manager = ScriptableObject.CreateInstance<GameFlowManager>();
            Directory.CreateDirectory(kDefaultConfigFolder);
            AssetDatabase.CreateAsset(manager, kPath);
            AddressableUtility.AddAddressableGroup(kPath, true);
            CreateScripts();
            CreateMemberElements();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        internal static void CreateScripts()
        {
            Directory.CreateDirectory(kDefaultConfigFolder + "/ElementScripts");
            var filePath = Path.Combine(Application.dataPath, "GameFlow/ElementScripts/GameFlowElements.asmdef");

            try
            {
                File.WriteAllText(filePath, kTemplateAsmdef);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error writing to file: " + e.Message);
            }
        }

        internal static void CreateMemberElements()
        {
            Directory.CreateDirectory(kDefaultConfigFolder + "/Elements");
        }
    }
}
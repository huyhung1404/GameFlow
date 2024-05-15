using GameFlow.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace GameFlow.Editor
{
    public static class GameFlowManagerObject
    {
        private static GameFlowManager _instance;
        public const string kDefaultConfigAssetName = "GameFlowManager";
        public const string kDefaultConfigFolder = "Assets/GameFlow";
        public const string kPath = kDefaultConfigFolder + "/" + kDefaultConfigAssetName + ".asset";

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
        }

        internal static void CreateMemberElements()
        {
            Directory.CreateDirectory(kDefaultConfigFolder + "/Elements");
        }
    }
}
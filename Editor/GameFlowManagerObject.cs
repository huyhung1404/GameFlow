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
            CreateTemplates();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        internal static void CreateScripts()
        {
            Directory.CreateDirectory(kDefaultConfigFolder + "/ElementScripts");
            CopyFromPackage("GameFlowElements", "ElementScripts", ".txt", ".asmdef");
        }

        internal static void CreateMemberElements()
        {
            Directory.CreateDirectory(kDefaultConfigFolder + "/Elements");
        }

        internal static void CreateTemplates()
        {
            Directory.CreateDirectory(kDefaultConfigFolder + "/Templates");
            CopyFromPackage("TemplateGameFlowElement", "Templates", ".prefab", ".prefab");
            CopyFromPackage("TemplateGameFlowElement", "Templates", ".unity", ".unity");
            CopyFromPackage("TemplateUserInterfaceFlowElement", "Templates", ".prefab", ".prefab");
            CopyFromPackage("TemplateUserInterfaceFlowElement", "Templates", ".unity", ".unity");
        }

        public static void CopyFromPackage(string fileName, string projectFolder, string packageExtension, string projectExtension)
        {
            var packagePath = "Packages/com.huyhung1404.gameflow/Templates/" + fileName + packageExtension;
            var projectPath = kDefaultConfigFolder + "/" + projectFolder + "/" + fileName + projectExtension;

            var projectDir = Path.GetDirectoryName(projectPath);
            if (!Directory.Exists(projectDir))
            {
                Directory.CreateDirectory(projectDir);
            }

            if (File.Exists(packagePath))
            {
                File.Copy(packagePath, projectPath, true);
            }
        }
    }
}
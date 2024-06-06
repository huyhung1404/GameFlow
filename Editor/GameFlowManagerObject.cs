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

        internal static GameFlowManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = AssetDatabase.LoadAssetAtPath<GameFlowManager>(PackagePath.ManagerPath());
                return _instance;
            }
        }

        internal static void CreateDefaultInstance()
        {
            var manager = ScriptableObject.CreateInstance<GameFlowManager>();
            Directory.CreateDirectory(PackagePath.ProjectFolderPath());
            AssetDatabase.CreateAsset(manager, PackagePath.ManagerPath());
            AddressableUtility.AddAddressableGroupController(PackagePath.ManagerPath());
            CreateScripts();
            CreateTemplates();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateScripts()
        {
            Directory.CreateDirectory(PackagePath.ScriptsGenerateFolderPath());
            CopyFromPackage("GameFlowElements", PackagePath.kScriptFolderName, ".txt", ".asmdef");
        }

        private static void CreateTemplates()
        {
            Directory.CreateDirectory(PackagePath.ProjectTemplatesPath());
            CopyFromPackage("TemplateGameFlowElement", "Templates", ".prefab", ".prefab");
            CopyFromPackage("TemplateGameFlowElement", "Templates", ".unity", ".unity");
            CopyFromPackage("TemplateUserInterfaceFlowElement", "Templates", ".prefab", ".prefab");
            CopyFromPackage("TemplateUserInterfaceFlowElement", "Templates", ".unity", ".unity");
            CopyFromPackage("TemplateScripts", "Templates", ".txt", ".txt");
        }

        private static void CopyFromPackage(string fileName, string projectFolder, string packageExtension, string projectExtension)
        {
            var packagePath = PackagePath.PackageTemplatesPath() + "/" + fileName + packageExtension;
            var projectPath = PackagePath.ProjectFolderPath() + "/" + projectFolder + "/" + fileName + projectExtension;

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
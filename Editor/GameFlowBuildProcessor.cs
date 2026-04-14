using System.IO;
using GameFlow.Internal;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GameFlow.Editor
{
    internal class GameFlowBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string k_resourceFolderPath = "Assets/Resources";

        private static string ConfigFilePath => $"{k_resourceFolderPath}/{PackagePath.k_ConfigResourceName}.txt";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var folderPath = Configs.instance.FolderPath;
            if (string.IsNullOrEmpty(folderPath)) return;

            if (!Directory.Exists(k_resourceFolderPath))
            {
                Directory.CreateDirectory(k_resourceFolderPath);
            }

            File.WriteAllText(ConfigFilePath, folderPath);
            AssetDatabase.ImportAsset(ConfigFilePath);
            Debug.Log($"[GameFlow] Build preprocessor: created {ConfigFilePath} with folder path \"{folderPath}\"");
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (!File.Exists(ConfigFilePath)) return;

            AssetDatabase.DeleteAsset(ConfigFilePath);

            var metaPath = ConfigFilePath + ".meta";
            if (File.Exists(metaPath))
            {
                File.Delete(metaPath);
            }

            if (Directory.Exists(k_resourceFolderPath)
                && Directory.GetFiles(k_resourceFolderPath).Length == 0
                && Directory.GetDirectories(k_resourceFolderPath).Length == 0)
            {
                AssetDatabase.DeleteAsset(k_resourceFolderPath);
            }

            AssetDatabase.Refresh();
            Debug.Log("[GameFlow] Build postprocessor: cleaned up temporary config resource.");
        }
    }
}

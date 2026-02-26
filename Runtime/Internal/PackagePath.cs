using UnityEditor;
using UnityEngine;

namespace GameFlow.Internal
{
    internal static class PackagePath
    {
        internal enum PathType
        {
            UnityPath,
            FullPath
        }

        private const string k_defaultConfigAssetName = "GameFlowManager";
        private const string k_defaultProjectFolderName = "GameFlow";
        internal const string k_ScriptFolderName = "ElementScripts";
        private const string k_assetsFolderName = "Elements";
        private const string k_assetsElementsFolderName = "GameFlowElements";
        private const string k_assetsUserInterfaceElementsFolderName = "UserInterfaceFlowElements";
        private const string k_assetSOFolderName = "ElementControllers";

        internal static string ProjectFolderPath(PathType type = PathType.UnityPath)
        {
#if UNITY_EDITOR
            var folderParentName = EditorPrefs.GetString("com.huyhung1404.gameflow.folderParentName", string.Empty);
            if (type == PathType.UnityPath)
            {
                return $"Assets/{folderParentName}{k_defaultProjectFolderName}";
            }

            return $"{Application.dataPath}/{folderParentName}{k_defaultProjectFolderName}";
#else
             if (type == PathType.UnityPath)
             {
                 return $"Assets/{k_defaultProjectFolderName}";
             }
 
             return $"{Application.dataPath}/{k_defaultProjectFolderName}";
#endif
        }

        internal static string ManagerPath(PathType type = PathType.UnityPath)
        {
            return $"{ProjectFolderPath(type)}/{k_defaultConfigAssetName}.asset";
        }

        internal static string ScriptsGenerateFolderPath(PathType type = PathType.UnityPath)
        {
            return ProjectFolderPath(type) + "/" + k_ScriptFolderName;
        }

        internal static string ProjectTemplatesPath(PathType type = PathType.UnityPath)
        {
            return ProjectFolderPath(type) + "/Templates";
        }

        internal static string ProjectTemplateScriptPath(PathType type = PathType.UnityPath)
        {
            return ProjectTemplatesPath(type) + "/TemplateScripts.txt";
        }

        private static string AssetsFolderPath(PathType type = PathType.UnityPath)
        {
            return ProjectFolderPath(type) + "/" + k_assetsFolderName;
        }

        internal static string AssetsScriptableObjectFolderPath(PathType type = PathType.UnityPath)
        {
            return ProjectFolderPath(type) + "/" + k_assetSOFolderName;
        }

        internal static string AssetsElementsFolderPath(PathType type = PathType.UnityPath)
        {
            return AssetsFolderPath(type) + "/" + k_assetsElementsFolderName;
        }

        internal static string AssetsUserInterfaceElementsFolderPath(PathType type = PathType.UnityPath)
        {
            return AssetsFolderPath(type) + "/" + k_assetsUserInterfaceElementsFolderName;
        }
    }
}
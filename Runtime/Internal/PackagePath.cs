﻿using UnityEditor;
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

        private const string kDefaultConfigAssetName = "GameFlowManager";
        private const string kDefaultProjectFolderName = "GameFlow";
        internal const string kScriptFolderName = "ElementScripts";
        private const string kAssetsFolderName = "Elements";
        private const string kAssetsElementsFolderName = "GameFlowElements";
        private const string kAssetsUserInterfaceElementsFolderName = "UserInterfaceFlowElements";
        private const string kAssetSOFolderName = "ElementControllers";

        internal static string ProjectFolderPath(PathType type = PathType.UnityPath)
        {
#if UNITY_EDITOR
            var folderParentName = EditorPrefs.GetString("com.huyhung1404.gameflow.folderParentName", string.Empty);
            if (type == PathType.UnityPath)
            {
                return $"Assets/{folderParentName}{kDefaultProjectFolderName}";
            }

            return $"{Application.dataPath}/{folderParentName}{kDefaultProjectFolderName}";
#else
             if (type == PathType.UnityPath)
             {
                 return $"Assets/{kDefaultProjectFolderName}";
             }
 
             return $"{Application.dataPath}/{kDefaultProjectFolderName}";
#endif
        }

        internal static string ManagerPath(PathType type = PathType.UnityPath)
        {
            return $"{ProjectFolderPath(type)}/{kDefaultConfigAssetName}.asset";
        }

        internal static string ScriptsGenerateFolderPath(PathType type = PathType.UnityPath)
        {
            return ProjectFolderPath(type) + "/" + kScriptFolderName;
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
            return ProjectFolderPath(type) + "/" + kAssetsFolderName;
        }

        internal static string AssetsScriptableObjectFolderPath(PathType type = PathType.UnityPath)
        {
            return ProjectFolderPath(type) + "/" + kAssetSOFolderName;
        }

        internal static string AssetsElementsFolderPath(PathType type = PathType.UnityPath)
        {
            return AssetsFolderPath(type) + "/" + kAssetsElementsFolderName;
        }

        internal static string AssetsUserInterfaceElementsFolderPath(PathType type = PathType.UnityPath)
        {
            return AssetsFolderPath(type) + "/" + kAssetsUserInterfaceElementsFolderName;
        }
    }
}
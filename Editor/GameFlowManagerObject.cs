using System.IO;
using GameFlow.Internal;
using UnityEditor;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

namespace GameFlow.Editor
{
    public static class GameFlowManagerObject
    {
        private static GameFlowManager s_instance;

        internal static GameFlowManager Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetDatabase.LoadAssetAtPath<GameFlowManager>(PackagePath.ManagerPath());
                return s_instance;
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
            CreateFile(ObjectTemplate.AssemblyDefinition);
        }

        private static void CreateTemplates()
        {
            Directory.CreateDirectory(PackagePath.ProjectTemplatesPath());
            CreateFile(ObjectTemplate.GameFlowElementPrefab);
            CreateFile(ObjectTemplate.GameFlowElementScene);
            CreateFile(ObjectTemplate.UserInterfaceGameFlowElementPrefab);
            CreateFile(ObjectTemplate.UserInterfaceGameFlowElementScene);
            CreateFile(ObjectTemplate.TemplateScripts);
        }

        private static void CreateFile(ObjectTemplate.Template template)
        {
            var projectPath = PackagePath.ProjectFolderPath() + "/" + template.folder + "/" + template.fileName;

            var projectDir = Path.GetDirectoryName(projectPath);
            if (!Directory.Exists(projectDir))
            {
                Directory.CreateDirectory(projectDir);
            }

            File.WriteAllText(projectPath, template.content);
        }
    }

    internal static class ObjectTemplate
    {
        internal const string k_TemplatePath = "Packages/com.huyhung1404.gameflow/Editor/Templates/";

        internal class Template
        {
            public string folder;
            public string fileName;
            public string content;
        }

        internal static readonly Template AssemblyDefinition = new Template
        {
            fileName = "GameFlowElements.asmdef",
            folder = PackagePath.k_ScriptFolderName,
            content = AssetDatabase.LoadAssetAtPath<TextAsset>(k_TemplatePath + "AssemblyDefinitionTemplate.txt").text
        };

        internal static readonly Template GameFlowElementPrefab = new Template
        {
            fileName = "TemplateGameFlowElement.prefab",
            folder = "Templates",
            content = AssetDatabase.LoadAssetAtPath<TextAsset>(k_TemplatePath + "GameFlowElementPrefabTemplate.txt").text
        };

        internal static readonly Template GameFlowElementScene = new Template
        {
            fileName = "TemplateGameFlowElement.unity",
            folder = "Templates",
            content = AssetDatabase.LoadAssetAtPath<TextAsset>(k_TemplatePath + "GameFlowElementSceneTemplate.txt").text
        };

        internal static readonly Template UserInterfaceGameFlowElementPrefab = new Template
        {
            fileName = "TemplateUserInterfaceFlowElement.prefab",
            folder = "Templates",
            content = AssetDatabase.LoadAssetAtPath<TextAsset>(k_TemplatePath + "UIGameFlowElementPrefabTemplate.txt").text
        };

        internal static readonly Template UserInterfaceGameFlowElementScene = new Template
        {
            fileName = "TemplateUserInterfaceFlowElement.unity",
            folder = "Templates",
            content = AssetDatabase.LoadAssetAtPath<TextAsset>(k_TemplatePath + "UIGameFlowElementSceneTemplate.txt").text
        };

        internal static readonly Template TemplateScripts = new Template
        {
            fileName = "TemplateScripts.txt",
            folder = "Templates",
            content = AssetDatabase.LoadAssetAtPath<TextAsset>(k_TemplatePath + "ScriptTemplate.txt").text
        };
    }
}
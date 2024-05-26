using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using GameFlow.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameFlow.Editor
{
    public static class EditorElementUtility
    {
        public static void CreateTemplateClone(string templatePath, string targetPath)
        {
            var folder = Path.GetDirectoryName(targetPath);
            var parentFolder = Path.GetDirectoryName(Path.GetDirectoryName(targetPath));

            if (!Directory.Exists(parentFolder))
            {
                if (parentFolder != null) Directory.CreateDirectory(parentFolder);
            }

            if (!Directory.Exists(folder))
            {
                if (folder != null) Directory.CreateDirectory(folder);
            }

            if (File.Exists(templatePath))
            {
                File.Copy(templatePath, targetPath, true);
            }
        }

        public static AssetReference GetAssetReferenceValue(this SerializedProperty property)
        {
            var manager = (GameFlowManager)property.serializedObject.targetObject;
            var index = ExtractArrayIndex(property.propertyPath);
            return index == null ? null : manager.elementCollection.GetIndex(index.Value)?.reference;
        }

        private static int? ExtractArrayIndex(string input)
        {
            const string pattern = @"Array.data\[(\d+)\].reference";
            var match = Regex.Match(input, pattern);
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            return null;
        }
    }
}
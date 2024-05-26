using System.IO;
using System.Reflection;
using UnityEditor;

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
        
        public static T GetValue<T>(this SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;
            string[] fieldNames = property.propertyPath.Split('.');

            foreach (string fieldName in fieldNames)
            {
                obj = GetFieldValue(obj, fieldName);
                if (obj == null)
                    return default(T);
            }

            return (T)obj;
        }

        private static object GetFieldValue(object obj, string fieldName)
        {
            if (obj == null)
                return null;

            FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field == null)
                return null;

            return field.GetValue(obj);
        }
    }
}
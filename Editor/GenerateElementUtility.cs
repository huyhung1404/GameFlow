using System.IO;

namespace GameFlow.Editor
{
    public static class GenerateElementUtility
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
    }
}
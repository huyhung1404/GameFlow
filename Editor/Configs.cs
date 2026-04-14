using GameFlow.Internal;
using UnityEditor;

namespace GameFlow.Editor
{
    [FilePath("ProjectSettings/GameFlow_Config.asset", FilePathAttribute.Location.ProjectFolder)]
    public class Configs : ScriptableSingleton<Configs>
    {
        public string FolderPath;
        public bool AddressableFolderUnlock;

        [InitializeOnLoadMethod]
        private static void SyncEditorFolderPath()
        {
            PackagePath.s_editorFolderPath = instance.FolderPath;
        }

        public void SaveData()
        {
            PackagePath.s_editorFolderPath = FolderPath;
            Save(true);
        }
    }
}
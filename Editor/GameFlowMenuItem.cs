using UnityEditor;
using UnityEngine;

namespace GameFlow.Editor
{
    public static class GameFlowMenuItem
    {
        [MenuItem("GameFlow/Manager", priority = 0)]
        public static void OpenManager()
        {
            GameFlowManagerEditorWindow.OpenWindow();
        }

        [MenuItem("GameFlow/Viewer", priority = 1)]
        public static void OpenViewerEditor()
        {
            GameFlowViewerEditorWindow.OpenWindow();
        }

        [MenuItem("GameFlow/Documentation")]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://github.com/huyhung1404/GameFlow");
        }
    }
}
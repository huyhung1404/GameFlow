using UnityEditor;
using UnityEngine;

namespace GameFlow.Editor
{
    public static class GameFlowMenuItem
    {
        [MenuItem("GameFlow/Manager")]
        public static void OpenManager()
        {
            GameFlowManagerEditorWindow.OpenWindow();
        }

        [MenuItem("GameFlow/Viewer")]
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
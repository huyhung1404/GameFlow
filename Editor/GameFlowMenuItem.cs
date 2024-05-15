using UnityEditor;
using UnityEngine;

namespace GameFlow.Editor
{
    public static class GameFlowMenuItem
    {
        [MenuItem("GameFlow/Game Flow Manager")]
        public static void OpenManager()
        {
        }

        [MenuItem("GameFlow/Documentation")]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://github.com/huyhung1404/GameFlow");
        }
    }
}
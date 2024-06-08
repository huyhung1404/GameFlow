using UnityEditor;
using UnityEngine;

namespace GameFlow.Editor
{
    [CustomEditor(typeof(GameFlowElement),true)]
    public class GameFlowElementCustomInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Space(10);
            if (GUILayout.Button("Open Editor Window"))
            {
                GameFlowManagerEditorWindow.OpenWindow();
            }
        }
    }
}
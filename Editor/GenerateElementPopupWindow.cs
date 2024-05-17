using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
    public class GenerateElementPopupWindow : PopupWindowContent
    {
        private const string UXML_PATH = "Packages/com.huyhung1404.gameflow/Editor/UXML/GenerateElementPopupWindow.uxml";

        private readonly bool isUserInterface;
        private readonly Action onClose;

        public GenerateElementPopupWindow(bool isUserInterface, Action onClose)
        {
            this.isUserInterface = isUserInterface;
            this.onClose = onClose;
        }


        public override Vector2 GetWindowSize()
        {
            return new Vector2(400, 300);
        }

        public override void OnGUI(Rect rect)
        {
        }

        public override void OnOpen()
        {
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            visualTreeAsset.CloneTree(editorWindow.rootVisualElement);
        }

        public override void OnClose()
        {
            onClose.Invoke();
        }
    }
}
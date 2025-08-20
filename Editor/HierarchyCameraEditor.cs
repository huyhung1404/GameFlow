using GameFlow.Component;
using GameFlow.Internal;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

namespace GameFlow.Editor
{
    [InitializeOnLoad]
    public class HierarchyCameraEditor : MonoBehaviour
    {
        static HierarchyCameraEditor()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyWindowChanged;
            EditorApplication.hierarchyChanged += OnHierarchyWindowChanged;
        }

        private static void OnHierarchyWindowChanged()
        {
            if (Application.isPlaying) return;
            var prefab = PrefabStageUtility.GetCurrentPrefabStage();
            var controller = prefab == null ? FindObjectsOfType<GameFlowUICanvas>() : prefab.prefabContentsRoot.GetComponentsInChildren<GameFlowUICanvas>();
            if (controller == null) return;
            foreach (var uiCanvas in controller)
            {
                var canvas = uiCanvas.GetCanvas();
                if (canvas == null) continue;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                var scale = canvas.GetComponent<CanvasScaler>();
                if (scale != null) scale.referenceResolution = AssetDatabase.LoadAssetAtPath<GameFlowManager>(PackagePath.ManagerPath()).referenceResolution;
                if (canvas.worldCamera != null) continue;
                canvas.worldCamera = InitializationCamera(uiCanvas);
            }
        }

        private static Camera InitializationCamera(UnityEngine.Component controller)
        {
            var gameObject = new GameObject("HideCamera");
            gameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            gameObject.transform.SetParent(controller.transform, true);
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            var camera = gameObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.cullingMask = LayerMask.GetMask("UI");
            camera.orthographic = true;
            camera.orthographicSize = 5;
            camera.nearClipPlane = 0.3f;
            camera.farClipPlane = 101;
            camera.depth = 50;
            return camera;
        }
    }
}
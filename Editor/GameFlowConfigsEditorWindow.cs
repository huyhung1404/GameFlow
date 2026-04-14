using UnityEditor;
using UnityEngine;

namespace GameFlow.Editor
{
    public class GameFlowConfigsEditorWindow : EditorWindow
    {
        private const int k_TextureScale = 4;
        private static readonly Vector2 k_MinSize = new Vector2(380, 180);
        private static readonly Vector2 k_ToggleSize = new Vector2(44, 22);

        private Texture2D _trackTexture;
        private Texture2D _knobTexture;
        private Texture2D _knobShadowTexture;

        public static void OpenWindow()
        {
            var window = GetWindow<GameFlowConfigsEditorWindow>("GameFlow Configs");
            window.minSize = k_MinSize;
            window.Show();
        }

        private void OnEnable()
        {
            var tw = (int)k_ToggleSize.x * k_TextureScale;
            var th = (int)k_ToggleSize.y * k_TextureScale;
            var knobPx = (int)(k_ToggleSize.y - 4) * k_TextureScale;

            _trackTexture = CreatePillTexture(tw, th, Color.white);
            _knobTexture = CreateCircleTexture(knobPx, Color.white);
            _knobShadowTexture = CreateCircleTexture(knobPx, new Color(0f, 0f, 0f, 0.25f));
        }

        private void OnDisable()
        {
            if (_trackTexture) DestroyImmediate(_trackTexture);
            if (_knobTexture) DestroyImmediate(_knobTexture);
            if (_knobShadowTexture) DestroyImmediate(_knobShadowTexture);
        }

        private void OnGUI()
        {
            var configs = Configs.instance;

            EditorGUILayout.Space(10);

            var headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter
            };
          
            EditorGUILayout.Space(5);
            DrawFolderPathSection(configs);
            EditorGUILayout.Space(15);
            DrawLockToggleSection(configs);
        }

        private void DrawFolderPathSection(Configs configs)
        {
            DrawSectionHeader("Folder Path");

            EditorGUILayout.BeginHorizontal();
            var prefixStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(0, 0, 2, 0)
            };
            EditorGUILayout.LabelField("Assets/", prefixStyle, GUILayout.Width(48));

            EditorGUI.BeginChangeCheck();
            var folderPath = EditorGUILayout.TextField(configs.FolderPath);
            if (EditorGUI.EndChangeCheck())
            {
                configs.FolderPath = folderPath;
                configs.SaveData();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawLockToggleSection(Configs configs)
        {
            DrawSectionHeader("Addressable Group");
            EditorGUILayout.Space(2);

            var isLocked = !configs.AddressableFolderUnlock;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(4);

            var statusColor = isLocked
                ? new Color(0.25f, 0.75f, 0.35f)
                : new Color(0.55f, 0.55f, 0.55f);
            var statusText = isLocked ? "Locked" : "Unlocked";

            var labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
            labelStyle.normal.textColor = statusColor;
            EditorGUILayout.LabelField(statusText, labelStyle, GUILayout.Width(80));

            GUILayout.FlexibleSpace();

            var toggleRect = GUILayoutUtility.GetRect(k_ToggleSize.x, k_ToggleSize.y, GUILayout.Width(k_ToggleSize.x));
            if (DrawCustomToggle(toggleRect, isLocked))
            {
                if (isLocked)
                    AddressableUtility.UnlockSystem();
                else
                    AddressableUtility.LockSystem();
                Repaint();
            }

            GUILayout.Space(4);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawSectionHeader(string title)
        {
            var style = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };
            EditorGUILayout.LabelField(title, style);

            var rect = GUILayoutUtility.GetRect(0, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
            EditorGUILayout.Space(5);
        }

        private bool DrawCustomToggle(Rect rect, bool value)
        {
            var trackColor = value
                ? new Color(0.25f, 0.75f, 0.35f)
                : new Color(0.4f, 0.4f, 0.4f);

            GUI.color = trackColor;
            GUI.DrawTexture(rect, _trackTexture, ScaleMode.StretchToFill);
            GUI.color = Color.white;

            var knobSize = rect.height - 4;
            var knobX = value ? rect.xMax - knobSize - 2 : rect.x + 2;
            var knobY = rect.y + 2;

            GUI.DrawTexture(new Rect(knobX, knobY + 1, knobSize, knobSize), _knobShadowTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(new Rect(knobX, knobY, knobSize, knobSize), _knobTexture, ScaleMode.StretchToFill);

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

            if (Event.current.type != EventType.MouseDown || !rect.Contains(Event.current.mousePosition)) return false;
            Event.current.Use();
            return true;
        }

        private static Texture2D CreatePillTexture(int width, int height, Color color)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            var radius = height * 0.5f;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var px = x + 0.5f;
                    var py = y + 0.5f;
                    float dist;

                    if (px < radius)
                        dist = Vector2.Distance(new Vector2(px, py), new Vector2(radius, radius));
                    else if (px > width - radius)
                        dist = Vector2.Distance(new Vector2(px, py), new Vector2(width - radius, radius));
                    else
                        dist = Mathf.Abs(py - radius);

                    var alpha = Mathf.Clamp01((radius - dist) * 1.2f);
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, alpha * color.a));
                }
            }

            texture.Apply();
            return texture;
        }

        private static Texture2D CreateCircleTexture(int size, Color color)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            var center = size * 0.5f;
            var radius = size * 0.5f;

            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    var dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(center, center));
                    var alpha = Mathf.Clamp01((radius - dist) * 1.2f);
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, alpha * color.a));
                }
            }

            texture.Apply();
            return texture;
        }
    }
}

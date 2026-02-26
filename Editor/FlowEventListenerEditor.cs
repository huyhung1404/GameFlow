using System;
using GameFlow.Component;
using UnityEditor;
using UnityEngine;

namespace GameFlow.Editor
{
    [CustomEditor(typeof(FlowEventListener))]
    public class FlowEventListenerEditor : UnityEditor.Editor
    {
        private SerializedProperty _elementProperty;
        private SerializedProperty _delegatesProperty;
        private GUIContent _iconToolbarMinus;
        private GUIContent _eventIDName;
        private GUIContent[] _eventTypes;
        private GUIContent _addButtonContent;

        private void OnEnable()
        {
            _elementProperty = serializedObject.FindProperty("Element");
            _delegatesProperty = serializedObject.FindProperty("Delegates");
            _addButtonContent = EditorGUIUtility.TrTextContent("Add New Event");
            _eventIDName = new GUIContent("");
            _iconToolbarMinus = new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus"))
            {
                tooltip = "Remove all events in this list."
            };

            var eventNames = Enum.GetNames(typeof(FlowEventListener.EventTriggerType));
            _eventTypes = new GUIContent[eventNames.Length];
            for (var i = 0; i < eventNames.Length; ++i)
            {
                _eventTypes[i] = new GUIContent(eventNames[i]);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_elementProperty, GUIContent.none);
            var toBeRemovedEntry = -1;

            EditorGUILayout.Space();
            var removeButtonSize = GUIStyle.none.CalcSize(_iconToolbarMinus);

            for (var i = 0; i < _delegatesProperty.arraySize; ++i)
            {
                var delegateProperty = _delegatesProperty.GetArrayElementAtIndex(i);
                var eventProperty = delegateProperty.FindPropertyRelative("eventID");
                var callbacksProperty = delegateProperty.FindPropertyRelative("callback");
                _eventIDName.text = eventProperty.enumDisplayNames[eventProperty.enumValueIndex];
                EditorGUILayout.PropertyField(callbacksProperty, _eventIDName);
                var callbackRect = GUILayoutUtility.GetLastRect();
                var removeButtonPos = new Rect(callbackRect.xMax - removeButtonSize.x - 8, callbackRect.y + 1, removeButtonSize.x, removeButtonSize.y);
                if (GUI.Button(removeButtonPos, _iconToolbarMinus, GUIStyle.none))
                {
                    toBeRemovedEntry = i;
                }

                EditorGUILayout.Space();
            }

            if (toBeRemovedEntry > -1)
            {
                RemoveEntry(toBeRemovedEntry);
            }

            if (serializedObject.FindProperty("element").objectReferenceValue != null)
            {
                var btPosition = GUILayoutUtility.GetRect(_addButtonContent, GUI.skin.button);
                const float addButtonWidth = 200f;
                btPosition.x += (btPosition.width - addButtonWidth) / 2;
                btPosition.width = addButtonWidth;
                if (GUI.Button(btPosition, _addButtonContent))
                {
                    ShowAddTriggerMenu();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveEntry(int toBeRemovedEntry)
        {
            _delegatesProperty.DeleteArrayElementAtIndex(toBeRemovedEntry);
        }

        private void ShowAddTriggerMenu()
        {
            var menu = new GenericMenu();
            var isUIFlowElement = serializedObject.FindProperty("element").objectReferenceValue is UIFlowElement;
            for (var i = 0; i < _eventTypes.Length; ++i)
            {
                var active = isUIFlowElement || i == 0 || i == 1 || i == 5;
                for (var p = 0; p < _delegatesProperty.arraySize; ++p)
                {
                    var delegateEntry = _delegatesProperty.GetArrayElementAtIndex(p);
                    var eventProperty = delegateEntry.FindPropertyRelative("eventID");
                    if (eventProperty.enumValueIndex == i)
                    {
                        active = false;
                    }
                }

                if (active)
                {
                    menu.AddItem(_eventTypes[i], false, OnAddNewSelected, i);
                }
                else
                {
                    menu.AddDisabledItem(_eventTypes[i]);
                }
            }

            menu.ShowAsContext();
            Event.current.Use();
        }

        private void OnAddNewSelected(object index)
        {
            serializedObject.Update();
            var selected = (FlowEventListener.EventTriggerType)index;
            var newEntry = selected switch
            {
                FlowEventListener.EventTriggerType.OnActive => (FlowEventListener.Entry)new FlowEventListener.OnActiveEntry { EventID = selected },
                FlowEventListener.EventTriggerType.OnActiveWithData => new FlowEventListener.OnActiveWithDataEntry { EventID = selected },
                FlowEventListener.EventTriggerType.OnShowCompleted => new FlowEventListener.OnShowCompletedEntry { EventID = selected },
                FlowEventListener.EventTriggerType.OnKeyBack => new FlowEventListener.OnKeyBackEntry { EventID = selected },
                FlowEventListener.EventTriggerType.OnReFocus => new FlowEventListener.OnReFocusEntry { EventID = selected },
                FlowEventListener.EventTriggerType.OnRelease => new FlowEventListener.OnReleaseEntry { EventID = selected },
                _ => null
            };
            
            if (newEntry == null) return;
            _delegatesProperty.arraySize++;
            var newElementProperty = _delegatesProperty.GetArrayElementAtIndex(_delegatesProperty.arraySize - 1);
            newElementProperty.managedReferenceValue = newEntry;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
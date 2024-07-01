using System;
using System.Collections.Generic;
using GameFlow.Component;
using UnityEditor;
using UnityEngine;

namespace GameFlow.Editor
{
    [CustomEditor(typeof(FlowEventListener))]
    public class FlowEventListenerEditor : UnityEditor.Editor
    {
        private SerializedProperty elementProperty;
        private SerializedProperty delegatesProperty;
        private GUIContent iconToolbarMinus;
        private GUIContent eventIDName;
        private GUIContent[] eventTypes;
        private GUIContent addButtonContent;

        private void OnEnable()
        {
            elementProperty = serializedObject.FindProperty("element");
            delegatesProperty = serializedObject.FindProperty("delegates");
            addButtonContent = EditorGUIUtility.TrTextContent("Add New Event");
            eventIDName = new GUIContent("");
            iconToolbarMinus = new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus"))
            {
                tooltip = "Remove all events in this list."
            };

            var eventNames = Enum.GetNames(typeof(FlowEventListener.EventTriggerType));
            eventTypes = new GUIContent[eventNames.Length];
            for (var i = 0; i < eventNames.Length; ++i)
            {
                eventTypes[i] = new GUIContent(eventNames[i]);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(elementProperty, GUIContent.none);
            var toBeRemovedEntry = -1;

            EditorGUILayout.Space();
            var removeButtonSize = GUIStyle.none.CalcSize(iconToolbarMinus);

            for (var i = 0; i < delegatesProperty.arraySize; ++i)
            {
                var delegateProperty = delegatesProperty.GetArrayElementAtIndex(i);
                var eventProperty = delegateProperty.FindPropertyRelative("eventID");
                var callbacksProperty = delegateProperty.FindPropertyRelative("callback");
                eventIDName.text = eventProperty.enumDisplayNames[eventProperty.enumValueIndex];
                EditorGUILayout.PropertyField(callbacksProperty, eventIDName);
                var callbackRect = GUILayoutUtility.GetLastRect();
                var removeButtonPos = new Rect(callbackRect.xMax - removeButtonSize.x - 8, callbackRect.y + 1, removeButtonSize.x, removeButtonSize.y);
                if (GUI.Button(removeButtonPos, iconToolbarMinus, GUIStyle.none))
                {
                    toBeRemovedEntry = i;
                }

                EditorGUILayout.Space();
            }

            if (toBeRemovedEntry > -1)
            {
                RemoveEntry(toBeRemovedEntry);
            }

            var btPosition = GUILayoutUtility.GetRect(addButtonContent, GUI.skin.button);
            const float addButtonWidth = 200f;
            btPosition.x += (btPosition.width - addButtonWidth) / 2;
            btPosition.width = addButtonWidth;
            if (GUI.Button(btPosition, addButtonContent))
            {
                ShowAddTriggerMenu();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveEntry(int toBeRemovedEntry)
        {
            delegatesProperty.DeleteArrayElementAtIndex(toBeRemovedEntry);
        }

        private void ShowAddTriggerMenu()
        {
            var menu = new GenericMenu();
            for (var i = 0; i < eventTypes.Length; ++i)
            {
                var active = true;
                for (var p = 0; p < delegatesProperty.arraySize; ++p)
                {
                    var delegateEntry = delegatesProperty.GetArrayElementAtIndex(p);
                    var eventProperty = delegateEntry.FindPropertyRelative("eventID");
                    if (eventProperty.enumValueIndex == i)
                    {
                        active = false;
                    }
                }

                if (active)
                {
                    menu.AddItem(eventTypes[i], false, OnAddNewSelected, i);
                }
                else
                {
                    menu.AddDisabledItem(eventTypes[i]);
                }
            }

            menu.ShowAsContext();
            Event.current.Use();
        }

        private void OnAddNewSelected(object index)
        {
            serializedObject.ApplyModifiedProperties();
            var selected = (int)index;
            var listener = (FlowEventListener)target;
            listener.delegates ??= new List<FlowEventListener.Entry>();
            switch ((FlowEventListener.EventTriggerType)selected)
            {
                case FlowEventListener.EventTriggerType.OnActive:
                    listener.delegates.Add(new FlowEventListener.OnActiveEntry
                    {
                        eventID = FlowEventListener.EventTriggerType.OnActive
                    });
                    break;
                case FlowEventListener.EventTriggerType.OnActiveWithData:
                    listener.delegates.Add(new FlowEventListener.OnActiveWithDataEntry
                    {
                        eventID = FlowEventListener.EventTriggerType.OnActiveWithData
                    });
                    break;
                case FlowEventListener.EventTriggerType.OnShowCompleted:
                    listener.delegates.Add(new FlowEventListener.OnShowCompletedEntry
                    {
                        eventID = FlowEventListener.EventTriggerType.OnShowCompleted
                    });
                    break;
                case FlowEventListener.EventTriggerType.OnKeyBack:
                    listener.delegates.Add(new FlowEventListener.OnKeyBackEntry
                    {
                        eventID = FlowEventListener.EventTriggerType.OnKeyBack
                    });
                    break;
                case FlowEventListener.EventTriggerType.OnReFocus:
                    listener.delegates.Add(new FlowEventListener.OnReFocusEntry
                    {
                        eventID = FlowEventListener.EventTriggerType.OnReFocus
                    });
                    break;
                case FlowEventListener.EventTriggerType.OnRelease:
                    listener.delegates.Add(new FlowEventListener.OnReleaseEntry
                    {
                        eventID = FlowEventListener.EventTriggerType.OnRelease
                    });
                    break;
                default:
                    return;
            }

            serializedObject.Update();
        }
    }
}
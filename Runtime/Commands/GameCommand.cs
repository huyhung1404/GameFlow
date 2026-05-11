using System;
using System.Collections;
using GameFlow.Internal;
using UnityEngine;

namespace GameFlow
{
    public static class GameCommand
    {
        internal static readonly Type s_UIElementType = typeof(UIFlowElement);

        public static AddCommand Add<T>() where T : GameFlowElement
        {
            return Add(typeof(T));
        }

        public static AddCommand Add(Type type)
        {
            return type.IsSubclassOf(s_UIElementType) ? new AddUICommand(type) : new AddGameFlowCommand(type);
        }

        public static LoadCommand Load<T>() where T : UIFlowElement
        {
            return Load(typeof(T));
        }

        public static LoadCommand Load(Type type)
        {
            return new LoadCommand(type);
        }

        public static ReleaseCommand Release<T>() where T : GameFlowElement
        {
            return Release(typeof(T));
        }

        public static ReleaseCommand Release(Type type)
        {
            return type.IsSubclassOf(s_UIElementType) ? new ReleaseUIElementCommand(type) : new ReleaseElementCommand(type);
        }

        public static BaseLoadingTypeController LoadingOn(int index)
        {
            return GameFlowContext.Current?.Loading?.LoadingOn(new LoadingId(index));
        }

        public static BaseLoadingTypeController LoadingOn(string id)
        {
            return GameFlowContext.Current?.Loading?.LoadingOn(new LoadingId(id));
        }

        public static BaseLoadingTypeController LoadingOff(int index)
        {
            return GameFlowContext.Current?.Loading?.LoadingOff(new LoadingId(index));
        }

        public static BaseLoadingTypeController LoadingOff(string id)
        {
            return GameFlowContext.Current?.Loading?.LoadingOff(new LoadingId(id));
        }

        public static BaseLoadingTypeController GetLoading(int index)
        {
            return GameFlowContext.Current?.Loading?.Get(new LoadingId(index));
        }

        public static BaseLoadingTypeController GetLoading(string id)
        {
            return GameFlowContext.Current?.Loading?.Get(new LoadingId(id));
        }

        public static void LockFlow()
        {
            GameFlowContext.Current?.RuntimeController?.SetLock(true);
        }

        public static void UnlockFlow()
        {
            GameFlowContext.Current?.RuntimeController?.SetLock(false);
        }

        public static void EnableKeyBack()
        {
            GameFlowContext.Current?.RuntimeController?.SetDisableKeyBack(false);
        }

        public static void DisableKeyBack()
        {
            GameFlowContext.Current?.RuntimeController?.SetDisableKeyBack(true);
        }
    }

    public static class FlowInfo
    {
        public static bool IsInitialized()
        {
            return GameFlowContext.Current?.RuntimeController != null;
        }

        public static int CurrentCanvasCount()
        {
            var context = GameFlowContext.Current;
            return context?.UIElementsRuntime.ElementsRuntime.Count ?? 0;
        }

        public static bool IsTopCanvas<T>() where T : UIFlowElement
        {
            if (CurrentCanvasCount() == 0) return false;
            var elements = GameFlowContext.Current.UIElementsRuntime.ElementsRuntime;
            return elements[^1].GetType() == typeof(T);
        }

        public static Canvas GetCanvas<T>() where T : UIFlowElement
        {
            var context = GameFlowContext.Current;
            if (context == null) return null;
            var elements = context.UIElementsRuntime.ElementsRuntime;
            if (elements.Count == 0) return null;
            var type = typeof(T);
            for (var i = elements.Count - 1; i >= 0; i--)
            {
                var element = elements[i];
                if (element.GetType() == type) return element.RuntimeInstance.GetComponent<Canvas>();
            }

            return null;
        }

        public static UIFlowElement TopElement()
        {
            var context = GameFlowContext.Current;
            if (context == null) return null;
            var elements = context.UIElementsRuntime.ElementsRuntime;
            return elements.Count == 0 ? null : elements[elements.Count - 1];
        }

        public static IEnumerator IEWaitingTargetTotalCanvas(int totalCanvas, int delayFrame, float timeoutSeconds = 30f)
        {
            var deadline = Time.realtimeSinceStartup + timeoutSeconds;
            while (CurrentCanvasCount() != totalCanvas)
            {
                if (Time.realtimeSinceStartup > deadline) yield break;
                for (var i = 0; i < delayFrame; i++) yield return null;
            }
        }

        public static IEnumerator IEWaitingTopCanvasIs<T>(int delayFrame, float timeoutSeconds = 30f) where T : UIFlowElement
        {
            var deadline = Time.realtimeSinceStartup + timeoutSeconds;
            while (!IsTopCanvas<T>())
            {
                if (Time.realtimeSinceStartup > deadline) yield break;
                for (var i = 0; i < delayFrame; i++) yield return null;
            }
        }

        public static Vector2 CanvasReferenceResolution()
        {
            return GameFlowContext.Current?.Manager.ReferenceResolution ?? Vector2.zero;
        }
    }

    public static class AddCommandBuilder
    {
        /// <summary>
        /// Set Loading Index
        /// </summary>
        /// <param name="command"></param>
        /// <param name="index">index less than 0 => loading type is none</param>
        /// <returns></returns>
        public static AddCommand LoadingID(this AddCommand command, int index)
        {
            command.LoadingId = index < 0 ? null : new LoadingId(index);
            return command;
        }

        /// <summary>
        /// Set Loading ID
        /// </summary>
        /// <param name="command"></param>
        /// <param name="id">id is null => loading type is none</param>
        /// <returns></returns>
        public static AddCommand LoadingID(this AddCommand command, string id)
        {
            command.LoadingId = string.IsNullOrEmpty(id) ? null : new LoadingId(id);
            return command;
        }

        public static AddCommand Preload(this AddCommand command)
        {
            command.IsPreload = true;
            return command;
        }

        public static AddCommand OnCompleted(this AddCommand command, OnAddCommandCompleted completed)
        {
            command.OnCompleted = completed;
            return command;
        }

        public static AddCommand ActiveData(this AddCommand command, object data)
        {
            command.ActiveData = data;
            return command;
        }

        /// <summary>
        /// Only support scene reference
        /// </summary>
        /// <param name="command"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static AddCommand GetActiveHandle(this AddCommand command, out ReferenceActiveHandle handle)
        {
            command.ActiveHandle ??= new ReferenceActiveHandle(command);
            handle = command.ActiveHandle;
            return command;
        }

        public static LoadCommand GetActiveHandle(this LoadCommand command, out ReferenceActiveHandle handle)
        {
            handle = command.ActiveHandle;
            command.AutoActive = false;
            return command;
        }
    }

    public static class ReleaseCommandBuilder
    {
        public static ReleaseCommand OnCompleted(this ReleaseCommand command, OnReleaseCommandCompleted completed)
        {
            command.OnCompleted = completed;
            return command;
        }

        public static ReleaseCommand IgnoreAnimationHide(this ReleaseCommand command)
        {
            command.IgnoreAnimationHide = true;
            return command;
        }
    }
}
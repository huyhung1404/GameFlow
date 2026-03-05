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

        public static BaseLoadingTypeController LoadingOn(int i)
        {
            return !LoadingController.s_IsInitialization ? null : LoadingController.Instance.LoadingOn(i);
        }

        public static BaseLoadingTypeController LoadingOff(int i)
        {
            return !LoadingController.s_IsInitialization ? null : LoadingController.Instance.LoadingOff(i);
        }

        public static BaseLoadingTypeController Get(int i)
        {
            return !LoadingController.s_IsInitialization ? null : LoadingController.Instance.Get(i);
        }

        public static void LockFlow()
        {
            GameFlowRuntimeController.SetLock(true);
        }

        public static void UnlockFlow()
        {
            GameFlowRuntimeController.SetLock(false);
        }

        public static void EnableKeyBack()
        {
            GameFlowRuntimeController.SetDisableKeyBack(false);
        }

        public static void DisableKeyBack()
        {
            GameFlowRuntimeController.SetDisableKeyBack(true);
        }
    }

    public static class FlowInfo
    {
        public static int CurrentCanvasCount()
        {
            return UIElementsRuntimeManager.ElementsRuntime.Count;
        }

        public static bool IsTopCanvas<T>() where T : UIFlowElement
        {
            if (CurrentCanvasCount() == 0) return false;
            return UIElementsRuntimeManager.ElementsRuntime[^1].GetType() == typeof(T);
        }

        public static UnityEngine.Canvas GetCanvas<T>() where T : UIFlowElement
        {
            if (CurrentCanvasCount() == 0) return null;
            var type = typeof(T);
            for (var i = UIElementsRuntimeManager.ElementsRuntime.Count - 1; i >= 0; i--)
            {
                var element = UIElementsRuntimeManager.ElementsRuntime[i];
                if (element.GetType() == type) return element.RuntimeInstance.GetComponent<UnityEngine.Canvas>();
            }

            return null;
        }

        public static UIFlowElement TopElement()
        {
            return CurrentCanvasCount() == 0 ? null : UIElementsRuntimeManager.ElementsRuntime[CurrentCanvasCount() - 1];
        }

        public static IEnumerator IEWaitingTargetTotalCanvas(int totalCanvas, int delayFrame, float timeoutSeconds = 30f)
        {
            var deadline = UnityEngine.Time.realtimeSinceStartup + timeoutSeconds;
            while (CurrentCanvasCount() != totalCanvas)
            {
                if (UnityEngine.Time.realtimeSinceStartup > deadline) yield break;
                for (var i = 0; i < delayFrame; i++) yield return null;
            }
        }

        public static IEnumerator IEWaitingTopCanvasIs<T>(int delayFrame, float timeoutSeconds = 30f) where T : UIFlowElement
        {
            var deadline = UnityEngine.Time.realtimeSinceStartup + timeoutSeconds;
            while (!IsTopCanvas<T>())
            {
                if (UnityEngine.Time.realtimeSinceStartup > deadline) yield break;
                for (var i = 0; i < delayFrame; i++) yield return null;
            }
        }

        public static Vector2 CanvasReferenceResolution()
        {
            return GameFlowRuntimeController.Manager().ReferenceResolution;
        }
    }

    public static class AddCommandBuilder
    {
        /// <summary>
        /// Set Loading ID
        /// </summary>
        /// <param name="command"></param>
        /// <param name="id">id less than 0 => loading type is none</param>
        /// <returns></returns>
        public static AddCommand LoadingID(this AddCommand command, int id)
        {
            command.LoadingId = id;
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
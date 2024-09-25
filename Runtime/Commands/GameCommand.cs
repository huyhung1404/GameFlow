using System;
using GameFlow.Internal;

namespace GameFlow
{
    public static class GameCommand
    {
        internal static readonly Type UIElementType = typeof(UIFlowElement);

        public static AddCommand Add<T>() where T : GameFlowElement
        {
            var type = typeof(T);
            return type.IsSubclassOf(UIElementType) ? new AddUICommand(type) : new AddGameFlowCommand(type);
        }

        public static LoadCommand Load<T>() where T : UIFlowElement
        {
            return new LoadCommand(typeof(T));
        }

        public static ReleaseCommand Release<T>() where T : GameFlowElement
        {
            return Release(typeof(T));
        }

        internal static ReleaseCommand Release(Type type)
        {
            return type.IsSubclassOf(UIElementType) ? new ReleaseUIElementCommand(type) : new ReleaseElementCommand(type);
        }

        public static BaseLoadingTypeController LoadingOn(int i)
        {
            return !LoadingController.isInitialization ? null : LoadingController.instance.LoadingOn(i);
        }

        public static BaseLoadingTypeController LoadingOff(int i)
        {
            return !LoadingController.isInitialization ? null : LoadingController.instance.LoadingOff(i);
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
            return UIElementsRuntimeManager.elementsRuntime.Count;
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
            command.loadingId = id;
            return command;
        }

        public static AddCommand Preload(this AddCommand command)
        {
            command.isPreload = true;
            return command;
        }

        public static AddCommand OnCompleted(this AddCommand command, OnAddCommandCompleted completed)
        {
            command.onCompleted = completed;
            return command;
        }

        public static AddCommand ActiveData(this AddCommand command, object data)
        {
            command.activeData = data;
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
            command.activeHandle ??= new ReferenceActiveHandle(command);
            handle = command.activeHandle;
            return command;
        }

        public static LoadCommand GetActiveHandle(this LoadCommand command, out ReferenceActiveHandle handle)
        {
            handle = command.activeHandle;
            command.autoActive = false;
            return command;
        }
    }

    public static class ReleaseCommandBuilder
    {
        public static ReleaseCommand OnCompleted(this ReleaseCommand command, OnReleaseCommandCompleted completed)
        {
            command.onCompleted = completed;
            return command;
        }

        public static ReleaseCommand IgnoreAnimationHide(this ReleaseCommand command)
        {
            command.ignoreAnimationHide = true;
            return command;
        }
    }
}
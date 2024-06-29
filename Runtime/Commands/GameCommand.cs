using System;
using GameFlow.Internal;

namespace GameFlow
{
    public static class GameCommand
    {
        internal static readonly Type UIElementType = typeof(UserInterfaceFlowElement);

        public static AddCommand Add<T>() where T : GameFlowElement
        {
            var type = typeof(T);
            return type.IsSubclassOf(UIElementType) ? new AddUserInterfaceCommand(type) : new AddGameFlowCommand(type);
        }

        public static LoadCommand Load<T>() where T : UserInterfaceFlowElement
        {
            return new LoadCommand(typeof(T));
        }

        public static ReleaseCommand Release<T>() where T : GameFlowElement
        {
            var type = typeof(T);
            return type.IsSubclassOf(UIElementType) ? new ReleaseUserInterfaceElementCommand(type) : new ReleaseElementCommand(type);
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

        public static AddCommand SendData(this AddCommand command, object data)
        {
            command.sendData = data;
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
    }
}
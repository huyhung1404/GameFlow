using GameFlow.Internal;

namespace GameFlow
{
    internal class KeyBackCommand : ReleaseUserInterfaceElementCommand
    {
        public KeyBackCommand() : base(UIElementsRuntimeManager.GetTopElement())
        {
        }
    }
}
namespace GameFlow
{
    public static class GameCommand
    {
        public static AddCommand Add<T>(string id = null) where T : GameFlowElement
        {
            return new AddCommand(typeof(T), id);
        }

        public static LoadCommand Load<T>() where T : UserInterfaceFlowElement
        {
            return new LoadCommand(typeof(T));
        }

        public static ReleaseCommand Release<T>() where T : GameFlowElement
        {
            return new ReleaseCommand(typeof(T));
        }
    }
}
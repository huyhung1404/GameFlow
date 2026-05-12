namespace GameFlow
{
    public interface IFlowDataListener<in T>
    {
        void OnActiveWithData(T data);
    }
}

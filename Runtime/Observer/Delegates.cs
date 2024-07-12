namespace GameFlow
{
    public delegate void OnActive();

    public delegate void OnActiveWithData(object data);

    public delegate void OnRelease(bool releaseImmediately);

    public delegate void OnShowCompleted();

    public delegate void OnHide(ICommandReleaseHandle releaseHandle);

    public delegate void OnKeyBack();

    public delegate void OnReFocus();
}
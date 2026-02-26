using UnityEngine;

namespace GameFlow.Internal
{
    internal enum DrawType
    {
        Element,
        Canvas,
        SafeView,
        OnKeyBack
    }

    internal class InternalDrawAttribute : PropertyAttribute
    {
        internal DrawType DrawType { get; private set; }

        internal InternalDrawAttribute(DrawType drawType)
        {
            DrawType = drawType;
        }
    }
}
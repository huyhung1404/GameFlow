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
        internal DrawType drawType { get; private set; }

        internal InternalDrawAttribute(DrawType drawType)
        {
            this.drawType = drawType;
        }
    }
}
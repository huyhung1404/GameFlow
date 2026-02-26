using System;
using System.Collections.Generic;

namespace GameFlow.Internal
{
    public static class UIElementsRuntimeManager
    {
        internal static List<UIFlowElement> ElementsRuntime { get; }

        static UIElementsRuntimeManager()
        {
            ElementsRuntime = new List<UIFlowElement>();
        }

        internal static void AddUserInterfaceElement(UIFlowElement userInterfaceFlowElement)
        {
            userInterfaceFlowElement.CurrentSortingOrder = GetSortingOrder();
            ElementsRuntime.Add(userInterfaceFlowElement);
        }

        internal static Type GetTopElement()
        {
            var elementCount = ElementsRuntime.Count;
            return elementCount == 0 ? null : ElementsRuntime[elementCount - 1].ElementType;
        }

        internal static int GetSortingOrder()
        {
            var elementCount = ElementsRuntime.Count;
            if (elementCount == 0) return 0;
            return ElementsRuntime[elementCount - 1].CurrentSortingOrder + GameFlowRuntimeController.Manager().SortingOrderOffset;
        }

        internal static UIFlowElement GetElement(Type type)
        {
            for (var i = ElementsRuntime.Count - 1; i >= 0; i--)
            {
                if (ElementsRuntime[i].ElementType != type) continue;
                return ElementsRuntime[i];
            }

            return null;
        }

        internal static void RemoveElement(UIFlowElement element)
        {
            ElementsRuntime.Remove(element);
        }

        internal static void ReleaseAllElement(Action onReleaseCompleted)
        {
            var elementCount = ElementsRuntime.Count;
            if (elementCount == 0)
            {
                onReleaseCompleted?.Invoke();
                return;
            }

            var releaseCount = new ReleaseCount(onReleaseCompleted, elementCount);
            for (var i = ElementsRuntime.Count - 1; i >= 0; i--)
            {
                ReleaseElement(ElementsRuntime[i], releaseCount);
            }
        }

        private static void ReleaseElement(UIFlowElement element, ReleaseCount releaseCount)
        {
            FlowObservable.Event(element.GetType()).RaiseOnRelease(true);
            switch (element.ReleaseMode)
            {
                default:
                case ElementReleaseMode.ReleaseOnClose:
                case ElementReleaseMode.ReleaseOnCloseIncludeCallback:
                    element.Reference.ReleaseHandlePrefab(element.RuntimeInstance, releaseCount);
                    break;
                case ElementReleaseMode.NoneRelease:
                    element.RuntimeInstance.SetActive(false);
                    releaseCount.Count();
                    break;
            }
        }

        internal static void OnKeyBack()
        {
            var type = GetTopElement();
            if (type == null) return;
            FlowObservable.UIEvent(type).RaiseOnKeyBack();
        }

        internal class ReleaseCount : IReleaseCompleted
        {
            private readonly Action onCompleted;
            private readonly int totalCount;
            private int current;

            internal ReleaseCount(Action onCompleted, int totalCount)
            {
                this.onCompleted = onCompleted;
                this.totalCount = totalCount;
            }

            void IReleaseCompleted.UnloadCompleted(bool isSuccess)
            {
                Count();
            }

            internal void Count()
            {
                if (++current == totalCount) onCompleted.Invoke();
            }
        }
    }
}
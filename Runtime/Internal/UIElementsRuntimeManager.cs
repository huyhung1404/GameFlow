using System;
using System.Collections.Generic;

namespace GameFlow.Internal
{
    internal static class UIElementsRuntimeManager
    {
#if UNITY_EDITOR
        internal static List<UIFlowElement> elementsRuntime { get; }
#else
        private static List<UIFlowElement> elementsRuntime;
#endif

        static UIElementsRuntimeManager()
        {
            elementsRuntime = new List<UIFlowElement>();
        }

        internal static void AddUserInterfaceElement(UIFlowElement userInterfaceFlowElement)
        {
            userInterfaceFlowElement.currentSortingOrder = GetSortingOrder();
            elementsRuntime.Add(userInterfaceFlowElement);
        }

        internal static Type GetTopElement()
        {
            var elementCount = elementsRuntime.Count;
            return elementCount == 0 ? null : elementsRuntime[elementCount - 1].elementType;
        }

        internal static int GetSortingOrder()
        {
            var elementCount = elementsRuntime.Count;
            if (elementCount == 0) return 0;
            return elementsRuntime[elementCount - 1].currentSortingOrder + GameFlowRuntimeController.Manager().sortingOrderOffset;
        }

        internal static UIFlowElement GetElement(Type type)
        {
            for (var i = elementsRuntime.Count - 1; i >= 0; i--)
            {
                if (elementsRuntime[i].elementType != type) continue;
                return elementsRuntime[i];
            }

            return null;
        }

        internal static void RemoveElement(UIFlowElement element)
        {
            elementsRuntime.Remove(element);
        }

        internal static void ReleaseAllElement(Action onReleaseCompleted)
        {
            var elementCount = elementsRuntime.Count;
            var releaseCount = new ReleaseCount(onReleaseCompleted, elementCount);
            for (var i = elementsRuntime.Count - 1; i >= 0; i--)
            {
                ReleaseElement(elementsRuntime[i], releaseCount);
            }
        }

        private static void ReleaseElement(UIFlowElement element, ReleaseCount releaseCount)
        {
            FlowObservable.Event(element.GetType()).RaiseOnRelease(true);
            switch (element.releaseMode)
            {
                default:
                case ElementReleaseMode.RELEASE_ON_CLOSE:
                case ElementReleaseMode.RELEASE_ON_CLOSE_INCLUDE_CALLBACK:
                    element.reference.ReleaseHandlePrefab(element.runtimeInstance, releaseCount);
                    break;
                case ElementReleaseMode.NONE_RELEASE:
                    element.runtimeInstance.SetActive(false);
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